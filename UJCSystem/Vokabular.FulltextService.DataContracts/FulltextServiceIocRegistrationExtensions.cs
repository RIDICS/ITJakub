using Microsoft.Extensions.DependencyInjection;
using Vokabular.FulltextService.DataContracts.Clients;
using Vokabular.RestClient;

namespace Vokabular.FulltextService.DataContracts
{
    public static class FulltextServiceIocRegistrationExtensions
    {
        public static void RegisterFulltextServiceClientComponents(this IServiceCollection services,
            ServiceCommunicationConfiguration configuration = null)
        {
            if (configuration != null)
                services.AddSingleton(configuration);

            services.AddScoped<FulltextServiceClient>();
            services.AddScoped<FullRestClient>();
        }
    }
}