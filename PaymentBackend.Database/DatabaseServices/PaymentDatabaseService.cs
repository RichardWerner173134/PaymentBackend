using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using PaymentBackend.Settings;
using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.Database.DatabaseServices
{
    public interface IPaymentDatabaseService
    {
        Task<List<FullPaymentDto>> SelectAllPaymentsAsync(long paymentContext);
        Task<FullPaymentDto?> SelectPaymentByIdAsync(long paymentContext, long id);

        Task<List<FullPaymentDto>> SelectPaymentsByCreditorAsync(long paymentContext, string username);
        Task<List<FullPaymentDto>> SelectPaymentsByDebitorAsync(long paymentContext, string username);
        Task<List<FullPaymentDto>> SelectPaymentsByAuthorAsync(long paymentContext, string username);
        Task<long> MarkPaymentAsDeletedAsync(long paymentContext, long paymentId);
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

        public async Task<List<FullPaymentDto>> SelectAllPaymentsAsync(long paymentContext)
        {
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var joinedP2d = await SelectAllPayment2DebitorsAsync(connection, paymentContext);

            var result = MergeJoinedPayments(joinedP2d);

            await connection.CloseAsync();
            return result;
        }

        public async Task<FullPaymentDto?> SelectPaymentByIdAsync(long paymentContext, long id)
        {
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var joinedP2d = await SelectPaymentByIdAsync(connection, paymentContext, id);

            var result = MergeJoinedPayments(joinedP2d).FirstOrDefault();

            await connection.CloseAsync();
            return result;
        }

        public async Task<List<FullPaymentDto>> SelectPaymentsByCreditorAsync(long paymentContext, string username)
        {
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var joinedP2d = await SelectPaymentsByCreditorAsync(connection, paymentContext, username);

            var result = MergeJoinedPayments(joinedP2d);

            await connection.CloseAsync();
            return result;
        }

        public async Task<List<FullPaymentDto>> SelectPaymentsByDebitorAsync(long paymentContext, string username)
        {
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var joinedP2d = await SelectPaymentsByDebitorAsync(connection, paymentContext, username);

            var result = MergeJoinedPayments(joinedP2d);

            await connection.CloseAsync();
            return result;
        }

        public async Task<List<FullPaymentDto>> SelectPaymentsByAuthorAsync(long paymentContext, string username)
        {
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var joinedP2d = await SelectPaymentsByAuthorAsync(connection, paymentContext, username);

            var result = MergeJoinedPayments(joinedP2d);

            await connection.CloseAsync();
            return result;
        }

        public async Task<long> MarkPaymentAsDeletedAsync(long paymentContext, long paymentId)
        { 
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var result = await MarkPaymentAsDeletedAsync(connection, paymentContext, paymentId);

            await connection.CloseAsync();
            return result;
        }
    }
}
