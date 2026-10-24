using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using PaymentBackend.Settings;
using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.Database.DatabaseServices
{
    public interface IPaymentDatabaseService
    {
        Task<List<FullPaymentDto>> SelectAllPaymentsAsync(long paymentContext, CancellationToken cancellationToken);
        Task<FullPaymentDto?> SelectPaymentByIdAsync(long paymentContext, long id, CancellationToken cancellationToken);

        Task<List<FullPaymentDto>> SelectPaymentsByCreditorAsync(long paymentContext, string username, CancellationToken cancellationToken);
        Task<List<FullPaymentDto>> SelectPaymentsByDebitorAsync(long paymentContext, string username, CancellationToken cancellationToken);
        Task<List<FullPaymentDto>> SelectPaymentsByAuthorAsync(long paymentContext, string username, CancellationToken cancellationToken);
        Task<long> MarkPaymentAsDeletedAsync(long paymentContext, long paymentId, CancellationToken cancellationToken);
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

        public async Task<List<FullPaymentDto>> SelectAllPaymentsAsync(long paymentContext, CancellationToken cancellationToken)
        {
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            var joinedP2d = await SelectAllPayment2DebitorsAsync(connection, paymentContext, cancellationToken);

            var result = MergeJoinedPayments(joinedP2d);

            await connection.CloseAsync();
            return result;
        }

        public async Task<FullPaymentDto?> SelectPaymentByIdAsync(long paymentContext, long id, CancellationToken cancellationToken)
        {
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            var joinedP2d = await SelectPaymentByIdAsync(connection, paymentContext, id, cancellationToken);

            var result = MergeJoinedPayments(joinedP2d).FirstOrDefault();

            await connection.CloseAsync();
            return result;
        }

        public async Task<List<FullPaymentDto>> SelectPaymentsByCreditorAsync(long paymentContext, string username, CancellationToken cancellationToken)
        {
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            var joinedP2d = await SelectPaymentsByCreditorAsync(connection, paymentContext, username, cancellationToken);

            var result = MergeJoinedPayments(joinedP2d);

            await connection.CloseAsync();
            return result;
        }

        public async Task<List<FullPaymentDto>> SelectPaymentsByDebitorAsync(long paymentContext, string username, CancellationToken cancellationToken)
        {
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            var joinedP2d = await SelectPaymentsByDebitorAsync(connection, paymentContext, username, cancellationToken);

            var result = MergeJoinedPayments(joinedP2d);

            await connection.CloseAsync();
            return result;
        }

        public async Task<List<FullPaymentDto>> SelectPaymentsByAuthorAsync(long paymentContext, string username, CancellationToken cancellationToken)
        {
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            var joinedP2d = await SelectPaymentsByAuthorAsync(connection, paymentContext, username, cancellationToken);

            var result = MergeJoinedPayments(joinedP2d);

            await connection.CloseAsync();
            return result;
        }

        public async Task<long> MarkPaymentAsDeletedAsync(long paymentContext, long paymentId, CancellationToken cancellationToken)
        { 
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            var result = await MarkPaymentAsDeletedAsync(connection, paymentContext, paymentId, cancellationToken);

            await connection.CloseAsync();
            return result;
        }
    }
}
