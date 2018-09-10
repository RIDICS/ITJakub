using System;
using DryIoc;
using Microsoft.Extensions.DependencyInjection;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities
{
    public static class ForumDataEntitiesContainerRegistration
    {
        public static void AddForumDataEntities(this IRegistrator registrator)
        {
            registrator.Register<IUnitOfWork, UnitOfWork>(Reuse.InWebRequest, serviceKey: "forum");
            registrator.Register<CategoryRepository>(Reuse.InWebRequest, serviceKey: "forum");
            registrator.Register<ForumRepository>(Reuse.InWebRequest, serviceKey: "forum");
        }
    }
}
