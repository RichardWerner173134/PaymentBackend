using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using PaymentBackend.BL.Core;
using PaymentBackend.BL.Http;
using PaymentBackend.BL.Mapper;

namespace PaymentBackend.BL.DependencyInjection
{
    public class BlDiBUilder
    {
        public void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IUserResolver, UserResolver>();
            builder.Services.AddSingleton<IPaymentResolver, PaymentResolver>();
            builder.Services.AddSingleton<IPaymentForUserResolver, PaymentForUserResolver>();
            builder.Services.AddSingleton<IPaymentContextResolver, PaymentContextResolver>();

            builder.Services.AddSingleton<IFullPaymentDto2HttpPaymentMapper, FullPaymentDto2HttpPaymentMapper>();
            builder.Services.AddSingleton<IPaymentOverviewCalculator, PaymentOverviewCalculator>();
            builder.Services.AddSingleton<IBillCalculationService, BillCalculationService>();
            builder.Services.AddSingleton<IBillResolver, BillResolver>();
            builder.Services.AddSingleton<IBillHttpMapper, BillHttpMapper>();
        }
    }
}
