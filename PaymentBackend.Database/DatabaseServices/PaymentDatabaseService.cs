using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using PaymentBackend.Settings;
using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.Database.DatabaseServices
{
    public interface IPaymentDatabaseService
    {
        List<FullPaymentDto> SelectAllPayments();
        FullPaymentDto? SelectPaymentById(long id);

        List<FullPaymentDto> SelectPaymentsByCreditor(string username);
        List<FullPaymentDto> SelectPaymentsByDebitor(string username);
        List<FullPaymentDto> SelectPaymentsByAuthor(string username);
    }

    public class PaymentDatabaseService : AbstractPaymentDatabaseService, IPaymentDatabaseService
    {
        public PaymentDatabaseService(
            ISqlExceptionHandler exceptionHandler,
            IFunctionSettingsResolver functionSettingsResolver,
            ILogger<PaymentDatabaseService> logger
            ) : base(exceptionHandler,
                functionSettingsResolver,
                logger)
        {
        }

        public List<FullPaymentDto> SelectAllPayments()
        {
            return _exceptionHandler.ExecuteOrReturn(() =>
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                var joinedP2d = SelectAllPayment2Debitors(connection);

                var result = MergeJoinedPayments(joinedP2d);

                connection.Close();
                return result;
            }, new List<FullPaymentDto>());
        }

        public FullPaymentDto? SelectPaymentById(long id)
        {
            return _exceptionHandler.ExecuteOrReturn(() =>
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                var joinedP2d = SelectPaymentById(connection, id);

                var result = MergeJoinedPayments(joinedP2d).FirstOrDefault();

                connection.Close();
                return result;
            }, null);
        }

        public List<FullPaymentDto> SelectPaymentsByCreditor(string username)
        {
            return _exceptionHandler.ExecuteOrReturn(() =>
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                var joinedP2d = SelectPaymentsByCreditor(connection, username);

                var result = MergeJoinedPayments(joinedP2d);

                connection.Close();
                return result;
            }, new List<FullPaymentDto>());
        }

        public List<FullPaymentDto> SelectPaymentsByDebitor(string username)
        {
            return _exceptionHandler.ExecuteOrReturn(() =>
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                var joinedP2d = SelectPaymentsByDebitor(connection, username);

                var result = MergeJoinedPayments(joinedP2d);

                connection.Close();
                return result;
            }, new List<FullPaymentDto>());
        }

        public List<FullPaymentDto> SelectPaymentsByAuthor(string username)
        {
            return _exceptionHandler.ExecuteOrReturn(() =>
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                var joinedP2d = SelectPaymentsByAuthor(connection, username);

                var result = MergeJoinedPayments(joinedP2d);

                connection.Close();
                return result;
            }, new List<FullPaymentDto>());
        }
    }
}
