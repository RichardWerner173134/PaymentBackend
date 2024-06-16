using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Database.DependencyInjection
{
    public class DatabaseDiBuilder
    {
        public void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IUserDatabaseService, UserDatabaseService>();
            builder.Services.AddSingleton<ISqlExceptionHandler, SqlExceptionHandler>();
        }
    }
}
