using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class BoardRepository : ForumDbRepositoryBase
    {
        public BoardRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
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
