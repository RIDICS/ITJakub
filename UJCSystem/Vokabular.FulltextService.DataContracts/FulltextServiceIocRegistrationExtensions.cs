using Microsoft.Extensions.DependencyInjection;
using Vokabular.FulltextService.DataContracts.Clients;

namespace Vokabular.FulltextService.DataContracts
{
    public static class FulltextServiceIocRegistrationExtensions
    {
        public static void RegisterFulltextServiceClientComponents(this IServiceCollection services,
            FulltextServiceClientConfiguration configuration)
        {
            if (configuration != null)
                services.AddSingleton(configuration);

            services.AddScoped<FulltextServiceRestClient>();
            services.AddScoped<FulltextServiceClient>();
        }
    }
}