using System.Collections.Generic;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ProjectRepository : NHibernateDao
    {
        public ProjectRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual int GetProjectCount()
        {
            return GetSession().QueryOver<Project>()
                .RowCount();
        }

        public virtual IList<Project> GetProjectList(int start, int count)
        {
            return GetSession().QueryOver<Project>()
                .Fetch(x => x.CreatedByUser).Eager
                .OrderBy(x => x.Name).Asc
                .Skip(start)
                .Take(count)
                .List();
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
    }
}
