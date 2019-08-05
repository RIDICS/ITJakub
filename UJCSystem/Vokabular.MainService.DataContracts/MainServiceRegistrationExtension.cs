using Microsoft.Extensions.DependencyInjection;
using Vokabular.MainService.DataContracts.Clients;

namespace Vokabular.MainService.DataContracts
{
    public static class MainServiceRegistrationExtension
    {
        public static void RegisterMainServiceClient<TUriProvider, TTokenProvider>(this IServiceCollection services)
            where TUriProvider : class, IMainServiceUriProvider where TTokenProvider : class, IMainServiceAuthTokenProvider
        {
            services.AddScoped<IMainServiceUriProvider, TUriProvider>();
            services.AddScoped<IMainServiceAuthTokenProvider, TTokenProvider>();
            services.AddScoped<MainServiceRestClient>();
        }
    }
}