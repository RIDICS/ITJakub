using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class AccessMaskRepository : NHibernateDao
    {
        public AccessMaskRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual AccessMask GetAccessMaskByNameAndBoard(string name, Board board)
        {
            return GetSession().QueryOver<AccessMask>()
                .Where(x => x.Name == name)
                .Where(x => x.Board == board)
                .SingleOrDefault();
        }
    }
}