using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentBackend.BL.DependencyInjection;
using PaymentBackend.Common.DependencyInjection;
using PaymentBackend.Database.DependencyInjection;
using PaymentBackend.Settings.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaymentBackend.Functions
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(wa =>
                {
                    wa.Services.Configure<JsonSerializerOptions>(options =>
                    {
                        options.WriteIndented = true;
                        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                        options.Converters.Add(new JsonStringEnumConverter());
                    });
                })
                .ConfigureServices(s =>
                {
                    s.AddLogging();
                    s.AddApplicationInsightsTelemetryWorkerService();
                    s.ConfigureFunctionsApplicationInsights();

                    new SettingsDiBuilder().Configure(s);
                    new CommonDiBuilder().Configure(s);
                    new DatabaseDiBuilder().Configure(s);
                    new BlDiBUilder().Configure(s);
                })
                .Build();

            host.Run();
        }
    }
}
