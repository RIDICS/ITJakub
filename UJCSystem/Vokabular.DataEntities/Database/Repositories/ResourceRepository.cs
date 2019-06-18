using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ResourceRepository : NHibernateDao
    {
        public ResourceRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        public virtual BookVersionResource GetLatestBookVersion(long projectId)
        {
            Resource resourceAlias = null;

            return GetSession().QueryOver<BookVersionResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(x => resourceAlias.Project.Id == projectId && resourceAlias.LatestVersion.Id == x.Id)
                .SingleOrDefault();
        }

        public virtual IList<PageResource> GetProjectPages(long projectId)
        {
            Resource resourceAlias = null;

            return GetSession().QueryOver<PageResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(() => resourceAlias.Project.Id == projectId)
                .And(x => x.Id == resourceAlias.LatestVersion.Id)
                .Fetch(SelectMode.Fetch, x => x.Resource)
                .OrderBy(x => x.Position).Asc
                .List();
        }

        public virtual IList<TextResource> GetProjectTexts(long projectId, long? namedResourceGroupId, bool fetchParentPage)
        {
            Resource resourceAlias = null;

            var session = GetSession();

            if (fetchParentPage)
            {
                session.QueryOver<PageResource>()
                    .JoinAlias(x => x.Resource, () => resourceAlias)
                    .Where(x => x.Id == resourceAlias.LatestVersion.Id && resourceAlias.Project.Id == projectId)
                    .Fetch(SelectMode.Fetch, x => x.Resource)
                    .Future();
            }

            var query = session.QueryOver<TextResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(() => resourceAlias.Project.Id == projectId)
                .And(x => x.Id == resourceAlias.LatestVersion.Id)
                .Fetch(SelectMode.Fetch, x => x.Resource);

            if (namedResourceGroupId != null)
                query.And(() => resourceAlias.NamedResourceGroup.Id == namedResourceGroupId.Value);

            var result = query.Future();
            return result.ToList();
        }

        public virtual IList<ImageResource> GetProjectImages(long projectId, long? namedResourceGroupId, bool fetchParentPage)
        {
            Resource resourceAlias = null;

            var session = GetSession();

            if (fetchParentPage)
            {
                session.QueryOver<PageResource>()
                    .JoinAlias(x => x.Resource, () => resourceAlias)
                    .Where(x => x.Id == resourceAlias.LatestVersion.Id && resourceAlias.Project.Id == projectId)
                    .Fetch(SelectMode.Fetch, x => x.Resource)
                    .Future();
            }

            var query = session.QueryOver<ImageResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(() => resourceAlias.Project.Id == projectId)
                .And(x => x.Id == resourceAlias.LatestVersion.Id)
                .Fetch(SelectMode.Fetch, x => x.Resource);

            if (namedResourceGroupId != null)
            {
                query.And(() => resourceAlias.NamedResourceGroup.Id == namedResourceGroupId.Value);
            }

            var result = query.Future();
            return result.ToList();
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
                .Fetch(SelectMode.Fetch, x => x.Resource)
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
                .Fetch(SelectMode.Fetch, x => x.HeadwordItems)
                .OrderBy(() => headwordItemAlias.Headword).Asc
                .SingleOrDefault();
        }

        public virtual IList<HeadwordResource> GetProjectLatestHeadwordPage(long projectId, int start, int count)
        {
            Resource resourceAlias = null;
            
            return GetSession().QueryOver<HeadwordResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(x => resourceAlias.Project.Id == projectId && resourceAlias.LatestVersion.Id == x.Id)
                .OrderBy(x => x.ExternalId).Asc
                .Take(count)
                .Skip(start)
                .List();
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

        public virtual IList<AudioResource> GetProjectFullAudioResources(long projectId)
        {
            Resource resourceAlias = null;

            return GetSession().QueryOver<AudioResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(x => resourceAlias.Project.Id == projectId && resourceAlias.LatestVersion.Id == x.Id)
                .And(x => resourceAlias.ContentType == ContentTypeEnum.FullLiteraryWork)
                .List();
        }

        public virtual TextResource GetTextResource(long resourceId)
        {
            Resource resourceAlias = null;

            return GetSession().QueryOver<TextResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(x => x.Id == resourceAlias.LatestVersion.Id && resourceAlias.Id == resourceId)
                .Fetch(SelectMode.Fetch, x => x.BookVersion)
                .Fetch(SelectMode.Fetch, x => x.Resource)
                .Fetch(SelectMode.Fetch, x => x.Resource.Project)
                .SingleOrDefault();
        }

        public virtual T GetLatestResourceVersion<T>(long resourceId) where T : ResourceVersion
        {
            Resource resourceAlias = null;

            return GetSession().QueryOver<T>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(x => x.Id == resourceAlias.LatestVersion.Id && resourceAlias.Id == resourceId)
                .SingleOrDefault();
        }

        public virtual IList<TextComment> GetCommentsForText(long textId)
        {
            TextComment subcommentAlias = null;

            return GetSession().QueryOver<TextComment>()
                .JoinAlias(x => x.TextComments, () => subcommentAlias, JoinType.LeftOuterJoin)
                .OrderBy(x => x.CreateTime).Asc
                .OrderBy(() => subcommentAlias.CreateTime).Asc
                .Where(x => x.ResourceText.Id == textId)
                .Fetch(SelectMode.Fetch, x => x.CreatedByUser)
                .Fetch(SelectMode.Fetch, x => x.TextComments)
                .TransformUsing(Transformers.DistinctRootEntity)
                .List()
                .Where(x => x.ParentComment == null)
                .ToList();
        }

        public virtual IList<NamedResourceGroup> GetNamedResourceGroupList(long projectId, ResourceTypeEnum? filterResourceType)
        {
            Resource resourceAlias = null;

            var query = GetSession().QueryOver<NamedResourceGroup>()
                .Where(x => x.Project.Id == projectId)
                .OrderBy(x => x.Name).Asc
                .TransformUsing(Transformers.DistinctRootEntity);

            if (filterResourceType != null)
            {
                query.JoinAlias(x => x.Resources, () => resourceAlias)
                    .And(() => resourceAlias.ResourceType == filterResourceType.Value);
            }

            return query.List();
        }

        public virtual EditionNoteResource GetLatestEditionNote(long projectId)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;

            var result = GetSession().QueryOver<EditionNoteResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .Where(x => x.Id == resourceAlias.LatestVersion.Id && projectAlias.Id == projectId)
                .OrderBy(x => x.CreateTime).Desc
                .Fetch(SelectMode.Fetch, x => x.BookVersion)
                .Take(1)
                .SingleOrDefault();
            return result;
        }
    }
}