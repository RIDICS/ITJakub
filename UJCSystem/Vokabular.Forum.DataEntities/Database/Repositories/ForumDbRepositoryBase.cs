using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class ForumDbRepositoryBase : NHibernateDao
    {
        public const string ServiceKey = "forum";

        public ForumDbRepositoryBase(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider, ServiceKey)
        {
        }
    }
}