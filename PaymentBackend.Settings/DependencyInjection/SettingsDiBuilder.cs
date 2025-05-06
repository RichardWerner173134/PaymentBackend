using Microsoft.Extensions.DependencyInjection;

namespace PaymentBackend.Settings.DependencyInjection
{
    public class SettingsDiBuilder
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IFunctionSettingsResolver, FunctionSettingsResolver>();
        }
    }
}
