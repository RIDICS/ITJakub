using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Test.LiveMock
{
    public class MockUserRepository : UserRepository
    {
        public MockUserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override User GetUserByToken(string authorizationToken)
        {
            return GetSession().QueryOver<User>()
                .Where(x => x.FirstName == "Test" && x.LastName == "User")
                .OrderBy(x => x.CreateTime).Desc
                .Take(1)
                .SingleOrDefault();
        }
    }
}