using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class UserRepository : NHibernateDao
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public User GetUserByEmail(string email)
        {
            return GetSession().QueryOver<User>()
                .Where(x => x.Email == email)
                .SingleOrDefault();
        }
    }
}

