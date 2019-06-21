using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class MainDbRepositoryBase : NHibernateDao
    {
        public const string ServiceKey = "default";

        public MainDbRepositoryBase(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider, ServiceKey)
        {
        }
    }
}
