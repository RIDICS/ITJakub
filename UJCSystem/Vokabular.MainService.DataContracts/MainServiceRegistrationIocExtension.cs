using Microsoft.Extensions.DependencyInjection;
using Vokabular.MainService.DataContracts.Clients;

namespace Vokabular.MainService.DataContracts
{
    public static class MainServiceIocRegistrationExtension
    {
        public static void RegisterMainServiceClientComponents<TTokenProvider, TClientLocalization>(this IServiceCollection services,
            MainServiceClientConfiguration configuration)
            where TTokenProvider : class, IMainServiceAuthTokenProvider
            where TClientLocalization : class, IMainServiceClientLocalization
        {
            if (configuration != null)
                services.AddSingleton(configuration);

            services.AddSingleton<IMainServiceAuthTokenProvider, TTokenProvider>();
            services.AddSingleton<IMainServiceClientLocalization, TClientLocalization>();
            
            services.AddSingleton<MainServiceRestClient>();

            services.AddTransient<MainServiceBookClient>();
            services.AddTransient<MainServiceCardFileClient>();
            services.AddTransient<MainServiceCodeListClient>();
            services.AddTransient<MainServiceExternalRepositoryClient>();
            services.AddTransient<MainServiceFavoriteClient>();
            services.AddTransient<MainServiceFeedbackClient>();
            services.AddTransient<MainServiceFilteringExpressionSetClient>();
            services.AddTransient<MainServiceMetadataClient>();
            services.AddTransient<MainServiceNewsClient>();
            services.AddTransient<MainServicePermissionClient>();
            services.AddTransient<MainServiceProjectClient>();
            services.AddTransient<MainServiceResourceClient>();
            services.AddTransient<MainServiceRoleClient>();
            services.AddTransient<MainServiceSessionClient>();
            services.AddTransient<MainServiceSnapshotClient>();
            services.AddTransient<MainServiceTermClient>();
            services.AddTransient<MainServiceUserClient>();
        }
    }
}