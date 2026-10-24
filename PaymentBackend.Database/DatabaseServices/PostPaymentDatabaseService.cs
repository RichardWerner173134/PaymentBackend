using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using PaymentBackend.Common.Model.Dto;
using PaymentBackend.Settings;

namespace PaymentBackend.Database.DatabaseServices
{
    public interface IPostPaymentDatabaseService
    {
        Task<long> InsertPaymentAsync(InsertPaymentDto paymentDto);
    }

    public class PostPaymentDatabaseService : AbstractDatabaseService, IPostPaymentDatabaseService

    {
        public PostPaymentDatabaseService(
            IFunctionSettingsResolver functionSettingsResolver,
            ILogger<PostPaymentDatabaseService> logger
        ) 
            : base(functionSettingsResolver, logger)
        {
        }

        public async Task<long> InsertPaymentAsync(InsertPaymentDto paymentDto)
        {
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            await using SqlTransaction transaction = (SqlTransaction)await connection.BeginTransactionAsync(IsolationLevel.RepeatableRead);
            try
            {
                var result = await InsertPaymentAsync(connection, transaction, paymentDto);
                paymentDto.PaymentId = result;
                await InsertPayment2UsersAsync(connection, transaction, paymentDto);

                await transaction.CommitAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        private async Task<long> InsertPaymentAsync(SqlConnection connection, SqlTransaction transaction, InsertPaymentDto paymentDto)
        {
            string sql = @"
insert into Payments (CreditorIdFk, Price, PaymentDate, UpdateTime, Description, AuthorIdFk, IsDeleted, PaymentContextIdFk)
OUTPUT INSERTED.ID
values (@CreditorIdFk, @Price, @PaymentDate, @UpdateTime, @Description, @AuthorIdFk, @IsDeleted, @PaymentContextIdFk)
";

            await using SqlCommand cmd = new(sql, connection, transaction);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@Price", paymentDto.Price);
            cmd.Parameters.AddWithValue("@Description", paymentDto.Description);
            cmd.Parameters.AddWithValue("@CreditorIdFk", paymentDto.Creditor.Id);
            cmd.Parameters.AddWithValue("@AuthorIdFk", paymentDto.Author.Id);
            cmd.Parameters.AddWithValue("@IsDeleted", paymentDto.IsDeleted);
            cmd.Parameters.AddWithValue("@PaymentContextIdFk", paymentDto.PaymentContext);
            cmd.Parameters.Add("@PaymentDate", SqlDbType.DateTime2).Value = paymentDto.PaymentDate;
            cmd.Parameters.Add("@UpdateTime", SqlDbType.DateTime2).Value = paymentDto.UpdateTime;

            return (long)(await cmd.ExecuteScalarAsync())!;
        }

        private async Task InsertPayment2UsersAsync(SqlConnection connection, SqlTransaction transaction, InsertPaymentDto paymentDto)
        {
            string sql = @"
insert into Payment2Debitor (DebitorIdFk, PaymentIdFk)
values 
";

            string valueStatement = Enumerable
                .Range(0, paymentDto.Debitors.Count)
                .Select(index => "(@DebitorIdFk" + index + ", " + paymentDto.PaymentId + ")")
                .Aggregate((s1, s2) => s1 + ", " + s2);

            string fullInsertStatement = sql + valueStatement;

            await using SqlCommand cmd = new(fullInsertStatement, connection, transaction);
            cmd.CommandType = CommandType.Text;

            for (var index = 0; index < paymentDto.Debitors.Count; index++)
            {
                cmd.Parameters.AddWithValue("@DebitorIdFk" + index, paymentDto.Debitors[index].Id);
            }

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
