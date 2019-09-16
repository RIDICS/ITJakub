using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ProjectRepository : MainDbRepositoryBase
    {
        public ProjectRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public virtual ListWithTotalCountResult<Project> GetProjectList(int start, int count, ProjectTypeEnum? projectType,
            string filterByName = null)
        {
            var query = GetSession().QueryOver<Project>()
                .Fetch(SelectMode.Fetch, x => x.CreatedByUser);

            if (projectType != null)
            {
                query.Where(x => x.ProjectType == projectType.Value);
            }

            if (!string.IsNullOrEmpty(filterByName))
            {
                query.WhereRestrictionOn(x => x.Name).IsInsensitiveLike(filterByName, MatchMode.Anywhere);
            }

            query.OrderBy(x => x.Name).Asc
                .Skip(start)
                .Take(count);

            var list = query.Future();
            var totalCount = query.ToRowCountQuery().FutureValue<int>();

            return new ListWithTotalCountResult<Project>
            {
                List = list.ToList(),
                Count = totalCount.Value
            };
        }

        public virtual Project GetProject(long projectId)
        {
            return GetSession().QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .Fetch(SelectMode.Fetch, x => x.CreatedByUser)
                .SingleOrDefault();
        }

        public virtual IList<FullProjectImportLog> GetAllImportLogByExternalId(string projectExternalId, ProjectTypeEnum projectType)
        {
            Project projectAlias = null;

            return GetSession().QueryOver<FullProjectImportLog>()
                .JoinAlias(x => x.Project, () => projectAlias)
                .Where(x => projectAlias.ExternalId == projectExternalId && projectAlias.ProjectType == projectType)
                .List();
        }

        public virtual Project GetProjectByExternalId(string externalId, ProjectTypeEnum projectType)
        {
            return GetSession().QueryOver<Project>()
                .Where(x => x.ExternalId == externalId && x.ProjectType == projectType)
                .SingleOrDefault();
        }

        public virtual BookType GetBookTypeByEnum(BookTypeEnum bookTypeEnum)
        {
            return GetSession().QueryOver<BookType>()
                .Where(x => x.Type == bookTypeEnum)
                .SingleOrDefault();
        }

        public virtual Snapshot GetLatestSnapshot(long projectId)
        {
            return GetSession().QueryOver<Snapshot>()
                .Where(x => x.Project.Id == projectId)
                .Fetch(SelectMode.Fetch, x => x.Project)
                .OrderBy(x => x.VersionNumber).Desc
                .Take(1)
                .SingleOrDefault();
        }

        public virtual Project GetProjectWithKeywords(long projectId)
        {
            return GetSession().QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .Fetch(SelectMode.Fetch, x => x.Keywords)
                .SingleOrDefault();
        }

        public virtual IList<ProjectOriginalAuthor> GetProjectOriginalAuthorList(long projectId, bool includeAuthors = false)
        {
            var query = GetSession().QueryOver<ProjectOriginalAuthor>()
                .Where(x => x.Project.Id == projectId)
                .OrderBy(x => x.Sequence).Asc;

            if (includeAuthors)
            {
                query.Fetch(SelectMode.Fetch, x => x.OriginalAuthor);
            }

            return query.List();
        }

        public virtual IList<ProjectResponsiblePerson> GetProjectResponsibleList(long projectId)
        {
            return GetSession().QueryOver<ProjectResponsiblePerson>()
                .Where(x => x.Project.Id == projectId)
                .Fetch(SelectMode.Fetch, x => x.ResponsiblePerson)
                .Fetch(SelectMode.Fetch, x => x.ResponsibleType)
                .List();
        }

        public virtual IList<LiteraryKind> GetProjectLiteraryKinds(long projectId)
        {
            Project projectAlias = null;

            return GetSession().QueryOver<LiteraryKind>()
                .JoinAlias(x => x.Projects, () => projectAlias)
                .Where(() => projectAlias.Id == projectId)
                .List();
        }

        public virtual IList<LiteraryGenre> GetProjectLiteraryGenres(long projectId)
        {
            Project projectAlias = null;

            return GetSession().QueryOver<LiteraryGenre>()
                .JoinAlias(x => x.Projects, () => projectAlias)
                .Where(() => projectAlias.Id == projectId)
                .List();
        }

        public virtual IList<LiteraryOriginal> GetProjectLiteraryOriginals(long projectId)
        {
            Project projectAlias = null;

            return GetSession().QueryOver<LiteraryOriginal>()
                .JoinAlias(x => x.Projects, () => projectAlias)
                .Where(() => projectAlias.Id == projectId)
                .List();
        }

        public virtual IList<Category> GetProjectCategories(long projectId)
        {
            Project projectAlias = null;

            return GetSession().QueryOver<Category>()
                .JoinAlias(x => x.Projects, () => projectAlias)
                .Where(() => projectAlias.Id == projectId)
                .List();
        }
    }
}