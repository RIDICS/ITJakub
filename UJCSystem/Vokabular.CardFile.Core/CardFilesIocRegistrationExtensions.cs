using Microsoft.Extensions.DependencyInjection;

namespace Vokabular.CardFile.Core
{
    public static class CardFilesIocRegistrationExtensions
    {
        public static void RegisterCardFileClientComponents(this IServiceCollection services,
            CardFilesCommunicationConfiguration configuration = null)
        {
            if (configuration != null)
                services.AddSingleton(configuration);

            services.AddScoped<CardFilesRestClient>();
            services.AddScoped<CardFilesClient>();
        }
    }
}
