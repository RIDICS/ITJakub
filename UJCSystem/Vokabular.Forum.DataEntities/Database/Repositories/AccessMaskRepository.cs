using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class AccessMaskRepository : ForumDbRepositoryBase
    {
        public AccessMaskRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public virtual AccessMask GetAccessMaskByNameAndBoard(string name, int boardId)
        {
            return GetSession().QueryOver<AccessMask>()
                .Where(x => x.Name == name)
                .Where(x => x.Board.BoardID == boardId)
                .SingleOrDefault();
        }
    }
}