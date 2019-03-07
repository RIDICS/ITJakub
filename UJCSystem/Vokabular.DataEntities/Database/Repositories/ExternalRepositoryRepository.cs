using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ExternalRepositoryRepository : NHibernateDao
    {
        public ExternalRepositoryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
      

        public virtual ExternalRepository GetExternalRepository(int externalRepositoryId)
        {
            return GetSession().QueryOver<ExternalRepository>()
                .Where(x => x.Id == externalRepositoryId)
                .SingleOrDefault();
        }
    }
}
