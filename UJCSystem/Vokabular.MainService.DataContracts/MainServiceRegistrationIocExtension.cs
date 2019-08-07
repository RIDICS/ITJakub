using Microsoft.Extensions.DependencyInjection;
using Vokabular.MainService.DataContracts.Clients;
using Vokabular.RestClient;

namespace Vokabular.MainService.DataContracts
{
    public static class MainServiceIocRegistrationExtension
    {
        public static void RegisterMainServiceClientComponents<TTokenProvider>(this IServiceCollection services,
            ServiceCommunicationConfiguration configuration = null)
            where TTokenProvider : class, IMainServiceAuthTokenProvider
        {
            if (configuration != null)
                services.AddSingleton(configuration);

            services.AddScoped<IMainServiceAuthTokenProvider, TTokenProvider>();
            services.AddScoped<MainServiceRestClient>();
        }
    }
}