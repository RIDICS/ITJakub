using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.FileProcessing.Service.Test.Mock
{
    public class MockPersonRepository : PersonRepository
    {
        public MockPersonRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public bool CanFindAuthorByName { get; set; }

        public override OriginalAuthor GetAuthorByName(string firstName, string lastName)
        {
            if (CanFindAuthorByName)
            {
                return new OriginalAuthor
                {
                    Id = 48,
                    FirstName = firstName,
                    LastName = lastName
                };
            }

            return null;
        }
    }
}
