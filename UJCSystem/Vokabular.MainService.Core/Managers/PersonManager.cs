using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Works.Person;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Managers
{
    public class PersonManager
    {
        private const int AutocompleteMaxCount = 5;
        private readonly PersonRepository m_personRepository;

        public PersonManager(PersonRepository personRepository)
        {
            m_personRepository = personRepository;
        }

        public int CreateOriginalAuthor(OriginalAuthorContract author)
        {
            var resultId = new CreateOriginalAuthorWork(m_personRepository, author).Execute();
            return resultId;
        }

        public int CreateResponsiblePerson(NewResponsiblePersonContract responsiblePerson)
        {
            var resultId = new CreateResponsiblePersonWork(m_personRepository, responsiblePerson).Execute();
            return resultId;
        }

        public int CreateResponsibleType(ResponsibleTypeContract responsibleType)
        {
            var resultId = new CreateResponsibleTypeWork(m_personRepository, responsibleType).Execute();
            return resultId;
        }

        public List<ResponsibleTypeContract> GetResponsibleTypeList()
        {
            var result = new GetResponsibleTypeListWork(m_personRepository).Execute();
            return Mapper.Map<List<ResponsibleTypeContract>>(result);
        }

        public List<OriginalAuthorContract> GetAuthorAutocomplete(string query)
        {
            var result = new GetAuthorAutocompleteWork(m_personRepository, query, AutocompleteMaxCount).Execute();
            return Mapper.Map<List<OriginalAuthorContract>>(result);
        }

        public List<ResponsiblePersonContract> GetResponsiblePersonAutocomplete(string query)
        {
            var result = new GetResponsiblePersonAutocompleteWork(m_personRepository, query, AutocompleteMaxCount).Execute();
            return Mapper.Map<List<ResponsiblePersonContract>>(result);
        }
    }
}