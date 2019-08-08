using Microsoft.Extensions.DependencyInjection;
using Vokabular.MainService.DataContracts.Clients;
using Vokabular.RestClient;

namespace Vokabular.MainService.DataContracts
{
    public static class MainServiceIocRegistrationExtension
    {
        public static void RegisterMainServiceClientComponents<TTokenProvider, TClientLocalization>(this IServiceCollection services,
            ServiceCommunicationConfiguration configuration = null)
            where TTokenProvider : class, IMainServiceAuthTokenProvider
            where TClientLocalization : class, IMainServiceClientLocalization
        {
            if (configuration != null)
                services.AddSingleton(configuration);

            services.AddScoped<IMainServiceAuthTokenProvider, TTokenProvider>();
            services.AddScoped<IMainServiceClientLocalization, TClientLocalization>();
            services.AddScoped<MainServiceRestClient>();
        }
    }
}