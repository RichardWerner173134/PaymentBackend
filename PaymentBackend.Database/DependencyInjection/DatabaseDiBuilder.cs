using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using PaymentBackend.Database.DatabaseServices;

namespace PaymentBackend.Database.DependencyInjection
{
    public class DatabaseDiBuilder
    {
        public void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IUserDatabaseService, UserDatabaseService>();
            builder.Services.AddSingleton<IPaymentDatabaseService, PaymentDatabaseService>();
            builder.Services.AddSingleton<IPostPaymentDatabaseService, PostPaymentDatabaseService>();
            builder.Services.AddSingleton<IPaymentContextDatabaseService, PaymentContextDatabaseService>();

            builder.Services.AddSingleton<ISqlExceptionHandler, SqlExceptionHandler>();
        }
    }
}
