using Microsoft.Extensions.DependencyInjection;
using Vokabular.MainService.DataContracts.Clients;

namespace Vokabular.MainService.DataContracts
{
    public static class MainServiceRegistrationExtension
    {
        public static void RegisterMainServiceClient<TTokenProvider>(this IServiceCollection services,
            MainServiceCommunicationConfiguration configuration = null)
            where TTokenProvider : class, IMainServiceAuthTokenProvider
        {
            if (configuration != null)
                services.AddSingleton(configuration);

            services.AddScoped<IMainServiceAuthTokenProvider, TTokenProvider>();
            services.AddScoped<MainServiceRestClient>();
        }
    }
}