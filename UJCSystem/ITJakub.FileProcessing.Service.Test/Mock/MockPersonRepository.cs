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

        public override ResponsiblePerson GetResponsiblePersonByName(string firstName, string lastName)
        {
            return new ResponsiblePerson
            {
                Id = 501,
                FirstName = firstName,
                LastName = lastName,
            };
        }

        public override ResponsibleType GetResponsibleTypeByName(string text)
        {
            return new ResponsibleType
            {
                Id = 12,
                Text = text,
            };
        }
    }
}
