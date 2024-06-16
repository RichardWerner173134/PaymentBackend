using BL.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace BL.DependencyInjection
{
    public class BlDiBUilder
    {
        public void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IUserResolver, UserResolver>();
        }
    }
}
