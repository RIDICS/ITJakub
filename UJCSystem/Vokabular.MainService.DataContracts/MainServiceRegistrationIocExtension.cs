using Microsoft.Extensions.DependencyInjection;
using Vokabular.MainService.DataContracts.Clients;

namespace Vokabular.MainService.DataContracts
{
    public static class MainServiceIocRegistrationExtension
    {
        public static void RegisterMainServiceClientComponents<TTokenProvider>(this IServiceCollection services,
            MainServiceClientConfiguration configuration)
            where TTokenProvider : class, IMainServiceAuthTokenProvider
        {
            if (configuration != null)
                services.AddSingleton(configuration);

            services.AddScoped<IMainServiceAuthTokenProvider, TTokenProvider>();
            services.AddSingleton<MainServiceRestClient>();

            services.AddScoped<MainServiceBookClient>();
            services.AddScoped<MainServiceCardFileClient>();
            services.AddScoped<MainServiceCodeListClient>();
            services.AddScoped<MainServiceExternalRepositoryClient>();
            services.AddScoped<MainServiceFavoriteClient>();
            services.AddScoped<MainServiceFeedbackClient>();
            services.AddScoped<MainServiceFilteringExpressionSetClient>();
            services.AddScoped<MainServiceMetadataClient>();
            services.AddScoped<MainServiceNewsClient>();
            services.AddScoped<MainServicePermissionClient>();
            services.AddScoped<MainServiceProjectClient>();
            services.AddScoped<MainServiceResourceClient>();
            services.AddScoped<MainServiceRoleClient>();
            services.AddScoped<MainServiceSessionClient>();
            services.AddScoped<MainServiceTermClient>();
            services.AddScoped<MainServiceUserClient>();
        }
    }
}