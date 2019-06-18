using Microsoft.Extensions.DependencyInjection;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.Shared.Container;

namespace Vokabular.ForumSite.DataEntities
{
    public class ForumDataEntitiesContainerRegistration : IContainerInstaller
    {
        public void Install(IServiceCollection services)
        {
            services.AddScoped<AccessMaskRepository>();
            services.AddScoped<BoardRepository>();
            services.AddScoped<CategoryRepository>();
            services.AddScoped<ForumAccessRepository>();
            services.AddScoped<ForumRepository>();
            services.AddScoped<GroupRepository>();
            services.AddScoped<MessageRepository>();
            services.AddScoped<TopicRepository>();
            services.AddScoped<UserRepository>();
        }
    }
}
