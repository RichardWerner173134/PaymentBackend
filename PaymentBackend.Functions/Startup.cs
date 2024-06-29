using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using PaymentBackend.BL.DependencyInjection;
using PaymentBackend.Common.DependencyInjection;
using PaymentBackend.Database.DependencyInjection;
using PaymentBackend.Functions;
using PaymentBackend.Settings.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace PaymentBackend.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();

            new SettingsDiBuilder().Configure(builder);
            new CommonDiBuilder().Configure(builder);
            // new HttpDiBuilder().Configure(builder);
            new BlDiBUilder().Configure(builder);
            new DatabaseDiBuilder().Configure(builder);
            new SettingsDiBuilder().Configure(builder);
        }

    }
}
