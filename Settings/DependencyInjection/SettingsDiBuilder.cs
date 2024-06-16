using Masterarbeit.OrderService;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Settings.DependencyInjection
{
    public class SettingsDiBuilder
    {
        public void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IFunctionSettingsResolver, FunctionSettingsResolver>();

        }
    }
}
