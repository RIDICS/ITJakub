using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    class BoardRepository : NHibernateDao
    {
        public BoardRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual Board GetBoardByName(string name)
        {
            return GetSession().QueryOver<Board>()
                .Where(x => x.Name == name)
                .SingleOrDefault();
        }
    }
}
