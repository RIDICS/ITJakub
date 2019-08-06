using Microsoft.Extensions.DependencyInjection;
using Vokabular.MainService.DataContracts.Clients;
using Vokabular.Shared.Container;

namespace Vokabular.MainService.DataContracts
{
    public class MainServiceClientContainerRegistration : IContainerInstaller
    {
        public void Install(IServiceCollection services)
        {
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