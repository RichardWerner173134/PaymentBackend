using BL.DependencyInjection;
using Common.DependencyInjection;
using Database.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Settings.DependencyInjection;
using TestingApp;

[assembly: FunctionsStartup(typeof(Startup))]
namespace TestingApp
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
        }

    }
}
