using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ProjectRepository : NHibernateDao
    {
        public ProjectRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        public virtual ListWithTotalCountResult<Project> GetProjectList(int start, int count)
        {
            var query = GetSession().QueryOver<Project>()
                .Fetch(x => x.CreatedByUser).Eager
                .OrderBy(x => x.Name).Asc
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
                .Fetch(x => x.CreatedByUser).Eager
                .SingleOrDefault();
        }

        public virtual IList<FullProjectImportLog> GetAllImportLogByExternalId(string projectExternalId)
        {
            Project projectAlias = null;

            return GetSession().QueryOver<FullProjectImportLog>()
                .JoinAlias(x => x.Project, () => projectAlias)
                .Where(x => projectAlias.ExternalId == projectExternalId)
                .List();
        }

        public virtual Project GetProjectByExternalId(string externalId)
        {
            return GetSession().QueryOver<Project>()
                .Where(x => x.ExternalId == externalId)
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
                .Fetch(x => x.Project).Eager
                .OrderBy(x => x.VersionNumber).Desc
                .Take(1)
                .SingleOrDefault();
        }

        public virtual Project GetProjectWithKeywords(long projectId)
        {
            return GetSession().QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .Fetch(x => x.Keywords).Eager
                .SingleOrDefault();
        }

        public virtual IList<ProjectOriginalAuthor> GetProjectOriginalAuthorList(long projectId, bool includeAuthors = false)
        {
            var query = GetSession().QueryOver<ProjectOriginalAuthor>()
                .Where(x => x.Project.Id == projectId)
                .OrderBy(x => x.Sequence).Asc;

            if (includeAuthors)
            {
                query.Fetch(x => x.OriginalAuthor);
            }

            return query.List();
        }
        
        public virtual IList<ProjectResponsiblePerson> GetProjectResponsibleList(long projectId)
        {
            return GetSession().QueryOver<ProjectResponsiblePerson>()
                .Where(x => x.Project.Id == projectId)
                .Fetch(x => x.ResponsiblePerson).Eager
                .Fetch(x => x.ResponsibleType).Eager
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
