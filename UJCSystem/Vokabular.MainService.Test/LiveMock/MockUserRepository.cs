using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Test.LiveMock
{
    public class MockUserRepository : UserRepository
    {
        public MockUserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}