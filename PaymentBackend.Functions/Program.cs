using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Trace;
using PaymentBackend.BL.DependencyInjection;
using PaymentBackend.Common.DependencyInjection;
using PaymentBackend.Database.DependencyInjection;
using PaymentBackend.Settings.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddOpenTelemetry()
    .UseFunctionsWorkerDefaults()
    .WithTracing(tracing =>
    {
        tracing
            .AddHttpClientInstrumentation()
            .AddSqlClientInstrumentation();
    })
    .UseAzureMonitorExporter(options =>
    {
        options.TracesPerSecond = 1;
        options.EnableTraceBasedLogsSampler = false;
    });

builder.Logging.SetMinimumLevel(LogLevel.Error);

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.WriteIndented = true;
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.Converters.Add(new JsonStringEnumConverter());
});

// dependency injection
new SettingsDiBuilder().Configure(builder.Services);
new CommonDiBuilder().Configure(builder.Services);
new DatabaseDiBuilder().Configure(builder.Services);
new BlDiBUilder().Configure(builder.Services);

builder.Build().Run();
