using Microsoft.Extensions.DependencyInjection;
using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.Core.Managers;
using Vokabular.ForumSite.Core.Works.Subworks;
using Vokabular.ForumSite.DataEntities;
using Vokabular.Shared.Container;

namespace Vokabular.ForumSite.Core
{
    public class ForumCoreContainerRegistration : IContainerInstaller
    {
        public void Install(IServiceCollection services)
        {
            services.AddScoped<ForumManager>();
            services.AddScoped<SubForumManager>();

            services.AddScoped<ForumAccessSubwork>();
            services.AddScoped<MessageSubwork>();

            services.AddScoped<ForumSiteUrlHelper>();
            services.AddScoped<VokabularUrlHelper>();
            services.AddScoped<ForumDefaultMessageGenerator>();

            new ForumDataEntitiesContainerRegistration().Install(services);
        }
    }
}
