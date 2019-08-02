using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class GroupRepository : ForumDbRepositoryBase
    {
        public GroupRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public virtual Group GetGroupByNameAndBoard(string name, int boardId)
        {
            return GetSession().QueryOver<Group>()
                .Where(x => x.Name == name)
                .Where(x => x.Board.BoardID == boardId)
                .SingleOrDefault();
        }
    }
}