using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using PaymentBackend.Settings;
using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.Database.DatabaseServices
{
    public interface IPaymentDatabaseService
    {
        List<FullPaymentDto> SelectAllPayments(long paymentContext);
        FullPaymentDto? SelectPaymentById(long paymentContext, long id);

        List<FullPaymentDto> SelectPaymentsByCreditor(long paymentContext, string username);
        List<FullPaymentDto> SelectPaymentsByDebitor(long paymentContext, string username);
        List<FullPaymentDto> SelectPaymentsByAuthor(long paymentContext, string username);
        long MarkPaymentAsDeleted(long paymentContext, long paymentId);
    }

    public class PaymentDatabaseService : AbstractPaymentDatabaseService, IPaymentDatabaseService
    {
        public PaymentDatabaseService(
            IFunctionSettingsResolver functionSettingsResolver,
            ILogger<PaymentDatabaseService> logger
        ) 
            : base(functionSettingsResolver, logger)
        {
        }

        public List<FullPaymentDto> SelectAllPayments(long paymentContext)
        {
            var connectionString = GetConnectionString();
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var joinedP2d = SelectAllPayment2Debitors(connection, paymentContext);

            var result = MergeJoinedPayments(joinedP2d);

            connection.Close();
            return result;
        }

        public FullPaymentDto? SelectPaymentById(long paymentContext, long id)
        {
            var connectionString = GetConnectionString();
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var joinedP2d = SelectPaymentById(connection, paymentContext, id);

            var result = MergeJoinedPayments(joinedP2d).FirstOrDefault();

            connection.Close();
            return result;
        }

        public List<FullPaymentDto> SelectPaymentsByCreditor(long paymentContext, string username)
        {
            var connectionString = GetConnectionString();
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var joinedP2d = SelectPaymentsByCreditor(connection, paymentContext, username);

            var result = MergeJoinedPayments(joinedP2d);

            connection.Close();
            return result;
        }

        public List<FullPaymentDto> SelectPaymentsByDebitor(long paymentContext, string username)
        {
            var connectionString = GetConnectionString();
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var joinedP2d = SelectPaymentsByDebitor(connection, paymentContext, username);

            var result = MergeJoinedPayments(joinedP2d);

            connection.Close();
            return result;
        }

        public List<FullPaymentDto> SelectPaymentsByAuthor(long paymentContext, string username)
        {
            var connectionString = GetConnectionString();
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var joinedP2d = SelectPaymentsByAuthor(connection, paymentContext, username);

            var result = MergeJoinedPayments(joinedP2d);

            connection.Close();
            return result;
        }

        public long MarkPaymentAsDeleted(long paymentContext, long paymentId)
        { 
            var connectionString = GetConnectionString();
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var result = MarkPaymentAsDeleted(connection, paymentContext, paymentId);

            connection.Close();
            return result;
        }
    }
}
