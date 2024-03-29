﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.Shared.Container;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities
{
    public class ForumDataEntitiesContainerRegistration : IContainerInstaller
    {
        public void Install(IServiceCollection services)
        {
            services.TryAddScoped<UnitOfWorkProvider>();

            services.AddScoped<AccessMaskRepository>();
            services.AddScoped<BoardRepository>();
            services.AddScoped<CategoryRepository>();
            services.AddScoped<ForumRepository>();
            services.AddScoped<GroupRepository>();
            services.AddScoped<UserRepository>();
        }
    }
}
