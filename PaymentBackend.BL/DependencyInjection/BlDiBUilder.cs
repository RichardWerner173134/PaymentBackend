using Microsoft.Extensions.DependencyInjection;
using PaymentBackend.BL.Core;
using PaymentBackend.BL.Http;
using PaymentBackend.BL.Mapper;

namespace PaymentBackend.BL.DependencyInjection
{
    public class BlDiBUilder
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IUserResolver, UserResolver>();
            serviceCollection.AddSingleton<IPaymentResolver, PaymentResolver>();
            serviceCollection.AddSingleton<IPaymentForUserResolver, PaymentForUserResolver>();
            serviceCollection.AddSingleton<IPaymentContextResolver, PaymentContextResolver>();
            
            serviceCollection.AddSingleton<IFullPaymentDto2HttpPaymentMapper, FullPaymentDto2HttpPaymentMapper>();
            serviceCollection.AddSingleton<IPaymentOverviewCalculator, PaymentOverviewCalculator>();
            serviceCollection.AddSingleton<IBillCalculationService, BillCalculationService>();
            serviceCollection.AddSingleton<IBillResolver, BillResolver>();
            serviceCollection.AddSingleton<IBillHttpMapper, BillHttpMapper>();
        }
    }
}
