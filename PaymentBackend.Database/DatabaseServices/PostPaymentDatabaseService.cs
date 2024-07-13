using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using PaymentBackend.Common.Model.Dto;
using PaymentBackend.Settings;

namespace PaymentBackend.Database.DatabaseServices
{
    public interface IPostPaymentDatabaseService
    {
        long InsertPayment(InsertPaymentDto paymentDto);
    }

    public class PostPaymentDatabaseService : AbstractDatabaseService, IPostPaymentDatabaseService

    {
        public PostPaymentDatabaseService(
            ISqlExceptionHandler exceptionHandler,
            IFunctionSettingsResolver functionSettingsResolver,
            ILogger<PostPaymentDatabaseService> logger) : base(exceptionHandler,
            functionSettingsResolver,
            logger)
        {
        }

        public long InsertPayment(InsertPaymentDto paymentDto)
        {
            var connectionString = GetConnectionString();
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.RepeatableRead);
            try
            {
                var result = InsertPayment(connection, transaction, paymentDto);
                paymentDto.PaymentId = result;
                InsertPayment2Users(connection, transaction, paymentDto);

                transaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                transaction.Rollback();
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        private long InsertPayment(SqlConnection connection, SqlTransaction transaction, InsertPaymentDto paymentDto)
        {
            string sql = @"
insert into Payments (CreditorIdFk, Price, PaymentDate, UpdateTime, Description, AuthorIdFk)
OUTPUT INSERTED.ID
values (@CreditorIdFk, @Price, @PaymentDate, @UpdateTime, @Description, @AuthorIdFk)
";

            using SqlCommand cmd = new(sql, connection, transaction);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@Price", paymentDto.Price);
            cmd.Parameters.AddWithValue("@Description", paymentDto.Description);
            cmd.Parameters.AddWithValue("@CreditorIdFk", paymentDto.Creditor.Id);
            cmd.Parameters.AddWithValue("@AuthorIdFk", paymentDto.Author.Id);
            cmd.Parameters.Add("@PaymentDate", SqlDbType.DateTime2).Value = paymentDto.PaymentDate;
            cmd.Parameters.Add("@UpdateTime", SqlDbType.DateTime2).Value = paymentDto.UpdateTime;

            return (long)cmd.ExecuteScalar();
        }

        private void InsertPayment2Users(SqlConnection connection, SqlTransaction transaction, InsertPaymentDto paymentDto)
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

            using SqlCommand cmd = new(fullInsertStatement, connection, transaction);
            cmd.CommandType = CommandType.Text;

            for (var index = 0; index < paymentDto.Debitors.Count; index++)
            {
                cmd.Parameters.AddWithValue("@DebitorIdFk" + index, paymentDto.Debitors[index].Id);
            }

            cmd.ExecuteNonQuery();
        }
    }
}
