using Microsoft.Extensions.DependencyInjection;
using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.Core.Managers;
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

            services.AddScoped<ForumSiteUrlHelper>();
            services.AddScoped<MessageGenerator>();

            new ForumDataEntitiesContainerRegistration().Install(services);
        }
    }
}
