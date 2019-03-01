using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ExternalResourceRepository : NHibernateDao
    {
        public ExternalResourceRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
      

        public virtual ExternalResource GetExternalResource(int externalResourceId)
        {
            return GetSession().QueryOver<ExternalResource>()
                .Where(x => x.Id == externalResourceId)
                .SingleOrDefault();
        }
    }
}
