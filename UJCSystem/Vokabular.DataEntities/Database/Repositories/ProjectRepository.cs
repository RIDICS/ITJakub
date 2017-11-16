using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.UnitOfWork;

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
    }
}
