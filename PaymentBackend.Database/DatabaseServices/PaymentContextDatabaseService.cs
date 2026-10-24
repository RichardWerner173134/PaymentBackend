using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using PaymentBackend.Common.Model;
using PaymentBackend.Settings;

namespace PaymentBackend.Database.DatabaseServices
{
    public interface IPaymentContextDatabaseService
    {
        Task<List<PaymentContext>> SelectAllPaymentContextsAsync();
        Task<PaymentContext?> SelectPaymentContextByIdAsync(long paymentContext);
    }

    public class PaymentContextDatabaseService : AbstractDatabaseService, IPaymentContextDatabaseService
    {
        public PaymentContextDatabaseService(
            IFunctionSettingsResolver functionSettingsResolver, 
            ILogger<UserDatabaseService> logger
        ) 
            : base(functionSettingsResolver, logger)
        {
        }

        public async Task<List<PaymentContext>> SelectAllPaymentContextsAsync()
        {
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var result = await SelectAllPaymentContextsAsync(connection);
            
            await connection.CloseAsync();
            return result;
        }

        public async Task<PaymentContext?> SelectPaymentContextByIdAsync(long paymentContext)
        {
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var result = await SelectPaymentContextByIdAsync(connection, paymentContext);

            await connection.CloseAsync();
            return result;
        }

        private async Task<List<PaymentContext>> SelectAllPaymentContextsAsync(SqlConnection connection)
        {
            string sql = @"
select 
    c.Id,
    c.ContextName,
    c.IsClosed
from 
    PaymentContext c
";

            await using SqlCommand cmd1 = new(sql, connection);
            cmd1.CommandType = CommandType.Text;

            List<PaymentContext> result = [];

            await using SqlDataReader reader = await cmd1.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var paymentContext = ConvertDbToObject(reader);
                result.Add(paymentContext);
            }

            return result;
        }

        private async Task<PaymentContext?> SelectPaymentContextByIdAsync(SqlConnection connection, long paymentContext)
        {
            string sql = @"
select 
    c.Id,
    c.ContextName,
    c.IsClosed
from 
    PaymentContext c
where
    c.Id = @PaymentContextId
";

            await using SqlCommand cmd1 = new(sql, connection);
            cmd1.CommandType = CommandType.Text;

            cmd1.Parameters.AddWithValue("@PaymentContextId", paymentContext);

            await using SqlDataReader reader = await cmd1.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                PaymentContext result = ConvertDbToObject(reader);
                return result;
            }

            return null;
        }

        private static PaymentContext ConvertDbToObject(SqlDataReader reader)
        {
            long? id = reader.SafeGetInt64("Id");
            string contextName = reader.SafeGetString("ContextName");
            bool isClosed = reader.SafeGetInt16("IsClosed") is 1;
            
            return new PaymentContext
            {
                Id = id!.Value,
                ContextName = contextName,
                IsClosed = isClosed,
            };
        }
    }
}
