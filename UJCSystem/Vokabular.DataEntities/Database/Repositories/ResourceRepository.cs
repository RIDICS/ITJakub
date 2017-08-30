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

        public virtual Term GetTermByExternalId(string externalId)
        {
            return GetSession().QueryOver<Term>()
                .Where(x => x.ExternalId == externalId)
                .SingleOrDefault();
        }

        public virtual TermCategory GetTermCategoryByName(string termCategoryName)
        {
            return GetSession().QueryOver<TermCategory>()
                .Where(x => x.Name == termCategoryName)
                .SingleOrDefault();
        }

        public virtual HeadwordResource GetLatestHeadword(long projectId, string externalId)
        {
            Resource resourceAlias = null;
            HeadwordItem headwordItemAlias = null;

            return GetSession().QueryOver<HeadwordResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(x => x.HeadwordItems, () => headwordItemAlias)
                .Where(x => resourceAlias.Project.Id == projectId && resourceAlias.LatestVersion.Id == x.Id)
                .And(x => x.ExternalId == externalId)
                .Fetch(x => x.HeadwordItems).Eager
                .OrderBy(() => headwordItemAlias.Headword).Asc
                .SingleOrDefault();
        }

        public virtual IList<TrackResource> GetProjectTracks(long projectId)
        {
            Resource resourceAlias = null;

            return GetSession().QueryOver<TrackResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(x => resourceAlias.Project.Id == projectId && resourceAlias.LatestVersion.Id == x.Id)
                .OrderBy(x => x.Position).Asc
                .List();
        }

        public virtual IList<AudioResource> GetProjectAudioResources(long projectId)
        {
            Resource resourceAlias = null;

            return GetSession().QueryOver<AudioResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(x => resourceAlias.Project.Id == projectId && resourceAlias.LatestVersion.Id == x.Id)
                .List();
        }
    }
}