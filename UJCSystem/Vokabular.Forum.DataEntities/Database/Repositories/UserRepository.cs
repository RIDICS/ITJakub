using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class UserRepository : ForumDbRepositoryBase
    {
        public UserRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public virtual User GetUserByEmail(string email)
        {
            return GetSession().QueryOver<User>()
                .Where(x => x.Email == email)
                .SingleOrDefault();
        }

        public virtual User GetUserByUserName(string username)
        {
            return GetSession().QueryOver<User>()
                .Where(x => x.Name == username)
                .SingleOrDefault();
        }
    }
}

