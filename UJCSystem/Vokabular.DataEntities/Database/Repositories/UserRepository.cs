using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class UserRepository : NHibernateDao
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public User GetUserByUsername(string username)
        {
            return GetSession().QueryOver<User>()
                .Where(x => x.UserName == username)
                .SingleOrDefault();
        }

        public User GetUserByToken(string authorizationToken)
        {
            return GetSession().QueryOver<User>()
                .Where(x => x.CommunicationToken == authorizationToken)
                .SingleOrDefault();
        }
    }
}
