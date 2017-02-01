using System.Collections.Generic;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ResourceRepository : NHibernateDao
    {
        public ResourceRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual IList<PageResource> GetProjectPages(long projectId)
        {
            Resource resourceAlias = null;

            return GetSession().QueryOver<PageResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(() => resourceAlias.Project.Id == projectId)
                .And(x => x.Id == resourceAlias.LatestVersion.Id)
                .Fetch(x => x.Resource).Eager
                .OrderBy(x => x.Position).Asc
                .List();
        }

        public virtual IList<TextResource> GetProjectTexts(long projectId, long namedResourceGroupId)
        {
            Resource resourceAlias = null;
            
            return GetSession().QueryOver<TextResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(() => resourceAlias.Project.Id == projectId && resourceAlias.NamedResourceGroup.Id == namedResourceGroupId)
                .And(x => x.Id == resourceAlias.LatestVersion.Id)
                .Fetch(x => x.Resource).Eager
                .List();
        }

        public virtual IList<ImageResource> GetProjectImages(long projectId, long namedResourceGroupId)
        {
            Resource resourceAlias = null;

            return GetSession().QueryOver<ImageResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(() => resourceAlias.Project.Id == projectId && resourceAlias.NamedResourceGroup.Id == namedResourceGroupId)
                .And(x => x.Id == resourceAlias.LatestVersion.Id)
                .Fetch(x => x.Resource).Eager
                .List();
        }

        public virtual NamedResourceGroup GetNamedResourceGroup(long projectId, string name, TextTypeEnum textType)
        {
            return GetSession().QueryOver<NamedResourceGroup>()
                .Where(x => x.Project.Id == projectId && x.TextType == textType && x.Name == name)
                .SingleOrDefault();
        }

        public virtual IList<ChapterResource> GetProjectChapters(long projectId)
        {
            Resource resourceAlias = null;

            return GetSession().QueryOver<ChapterResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(() => resourceAlias.Project.Id == projectId)
                .And(x => x.Id == resourceAlias.LatestVersion.Id)
                .Fetch(x => x.Resource).Eager
                .OrderBy(x => x.Position).Asc
                .List();
        }
    }
}