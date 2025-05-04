using System.Data;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using PaymentBackend.Common.Model;
using PaymentBackend.Settings;

namespace PaymentBackend.Database.DatabaseServices
{
    public interface IPaymentContextDatabaseService
    {
        List<PaymentContext> SelectAllPaymentContexts();
        PaymentContext? SelectPaymentContextById(long paymentContext);
    }

    public class PaymentContextDatabaseService : AbstractDatabaseService, IPaymentContextDatabaseService
    {
        public PaymentContextDatabaseService(
            ISqlExceptionHandler exceptionHandler,
            IFunctionSettingsResolver functionSettingsResolver, 
            ILogger<UserDatabaseService> logger
        ) 
            : base(exceptionHandler, functionSettingsResolver, logger)
        {
        }

        public List<PaymentContext> SelectAllPaymentContexts()
        {
            return _exceptionHandler.ExecuteOrThrow(() =>
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                var result = SelectAllPaymentContexts(connection);
                
                connection.Close();
                return result;
            });
        }

        public PaymentContext? SelectPaymentContextById(long paymentContext)
        {
            return _exceptionHandler.ExecuteOrThrow(() =>
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                var result = SelectPaymentContextById(connection, paymentContext);

                connection.Close();
                return result;
            });
        }

        private List<PaymentContext> SelectAllPaymentContexts(SqlConnection connection)
        {
            string sql = @"
select 
    c.Id,
    c.ContextName,
    c.IsClosed
from 
    PaymentContext c
";

            using SqlCommand cmd1 = new(sql, connection);
            cmd1.CommandType = CommandType.Text;

            List<PaymentContext> result = new();

            using SqlDataReader reader = cmd1.ExecuteReader();
            while (reader.Read())
            {
                var paymentContext = ConvertDbToObject(reader);
                result.Add(paymentContext);
            }

            return result;
        }

        private PaymentContext? SelectPaymentContextById(SqlConnection connection, long paymentContext)
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

            using SqlCommand cmd1 = new(sql, connection);
            cmd1.CommandType = CommandType.Text;

            cmd1.Parameters.AddWithValue("@PaymentContextId", paymentContext);

            using SqlDataReader reader = cmd1.ExecuteReader();
            while (reader.Read())
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
            
            return new PaymentContext()
            {
                Id = id!.Value,
                ContextName = contextName,
                IsClosed = isClosed,
            };
        }
    }
}
