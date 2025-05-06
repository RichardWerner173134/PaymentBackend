using Microsoft.Extensions.DependencyInjection;
using PaymentBackend.Database.DatabaseServices;

namespace PaymentBackend.Database.DependencyInjection
{
    public class DatabaseDiBuilder
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IUserDatabaseService, UserDatabaseService>();
            serviceCollection.AddSingleton<IPaymentDatabaseService, PaymentDatabaseService>();
            serviceCollection.AddSingleton<IPostPaymentDatabaseService, PostPaymentDatabaseService>();
            serviceCollection.AddSingleton<IPaymentContextDatabaseService, PaymentContextDatabaseService>();

            serviceCollection.AddSingleton<ISqlExceptionHandler, SqlExceptionHandler>();
        }
    }
}
