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

        public int GetProjectCount()
        {
            return GetSession().QueryOver<Project>()
                .RowCount();
        }

        public IList<Project> GetProjectList(int start, int count)
        {
            return GetSession().QueryOver<Project>()
                .Fetch(x => x.CreatedByUser).Eager
                .OrderBy(x => x.Name).Asc
                .Skip(start)
                .Take(count)
                .List();
        }

        public Project GetProject(long projectId)
        {
            return GetSession().QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .Fetch(x => x.CreatedByUser).Eager
                .SingleOrDefault();
        }

    }
}
