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
            services.AddScoped<MainServiceCategoryClient>();
            services.AddScoped<MainServiceExternalRepositoryClient>();
            services.AddScoped<MainServiceFavoriteClient>();
            services.AddScoped<MainServiceFeedbackClient>();
            services.AddScoped<MainServiceFilteringExpressionSetClient>();
            services.AddScoped<MainServiceMetadataClient>();
            services.AddScoped<MainServiceProjectClient>();
            services.AddScoped<MainServiceResourceClient>();
            services.AddScoped<MainServiceRoleClient>();
            services.AddScoped<MainServiceUserClient>();
        }
    }
}