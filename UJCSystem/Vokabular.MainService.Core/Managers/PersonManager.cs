using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Person;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class PersonManager
    {
        private readonly PersonRepository m_personRepository;
        private readonly IMapper m_mapper;

        public PersonManager(PersonRepository personRepository, IMapper mapper)
        {
            m_personRepository = personRepository;
            m_mapper = mapper;
        }

        #region OriginalAuthor CRUD

        public int CreateOriginalAuthor(OriginalAuthorContract author)
        {
            var resultId = new CreateOriginalAuthorWork(m_personRepository, author).Execute();
            return resultId;
        }

        public OriginalAuthorContract GetOriginalAuthor(int authorId)
        {
            var dbResult = m_personRepository.InvokeUnitOfWork(x => x.FindById<OriginalAuthor>(authorId));
            var result = m_mapper.Map<OriginalAuthorContract>(dbResult);
            return result;
        }

        public void UpdateOriginalAuthor(int authorId, OriginalAuthorContract author)
        {
            var updateOriginalAuthorWork = new UpdateOriginalAuthorWork(m_personRepository, authorId, author);
            updateOriginalAuthorWork.Execute();
        }

        public void DeleteOriginalAuthor(int authorId)
        {
            var deleteOriginalAuthorWork = new DeleteOriginalAuthorWork(m_personRepository, authorId);
            deleteOriginalAuthorWork.Execute();
        }

        #endregion

        #region ResponsiblePerson CRUD

        public int CreateResponsiblePerson(ResponsiblePersonContract responsiblePerson)
        {
            var resultId = new CreateOrUpdateResponsiblePersonWork(m_personRepository, null, responsiblePerson).Execute();
            return resultId;
        }

        public void UpdateResponsiblePerson(int responsiblePersonId, ResponsiblePersonContract data)
        {
            new CreateOrUpdateResponsiblePersonWork(m_personRepository, responsiblePersonId, data).Execute();
        }

        public void DeleteResponsiblePerson(int responsiblePersonId)
        {
            new DeleteResponsiblePersonWork(m_personRepository, responsiblePersonId).Execute();
        }

        public ResponsiblePersonContract GetResponsiblePerson(int responsiblePersonId)
        {
            var responsiblePerson = m_personRepository.InvokeUnitOfWork(x => x.FindById<ResponsiblePerson>(responsiblePersonId));
            return m_mapper.Map<ResponsiblePersonContract>(responsiblePerson);
        }

        #endregion


        public List<OriginalAuthorContract> GetAuthorAutocomplete(string query, BookTypeEnumContract? bookType, int? count)
        {
            if (query == null)
                query = string.Empty;

            var countValue = PagingHelper.GetAutocompleteCount(count);
            var bookTypeEnum = m_mapper.Map<BookTypeEnum?>(bookType);

            var result = m_personRepository.InvokeUnitOfWork(x => x.GetAuthorAutocomplete(query, bookTypeEnum, countValue));
            return m_mapper.Map<List<OriginalAuthorContract>>(result);
        }

        public List<ResponsiblePersonContract> GetResponsiblePersonAutocomplete(string query, int? count)
        {
            if (query == null)
                query = string.Empty;

            var countValue = PagingHelper.GetAutocompleteCount(count);

            var result = m_personRepository.InvokeUnitOfWork(x => x.GetResponsiblePersonAutocomplete(query, countValue));
            return m_mapper.Map<List<ResponsiblePersonContract>>(result);
        }

        public PagedResultList<ResponsiblePersonContract> GetResponsiblePersonList(int? start, int? count)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);
            var dbResult = m_personRepository.InvokeUnitOfWork(x => x.GetResponsiblePersonList(startValue, countValue));
            var resultList = m_mapper.Map<List<ResponsiblePersonContract>>(dbResult.List);

            return new PagedResultList<ResponsiblePersonContract>
            {
                List = resultList,
                TotalCount = dbResult.Count
            };
        }

        public PagedResultList<OriginalAuthorContract> GetOriginalAuthorList(int? start, int? count)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);
            var dbResult = m_personRepository.InvokeUnitOfWork(x => x.GetOriginalAuthorList(startValue, countValue));
            var resultList = m_mapper.Map<List<OriginalAuthorContract>>(dbResult.List);

            return new PagedResultList<OriginalAuthorContract>
            {
                List = resultList,
                TotalCount = dbResult.Count
            };
        }
    }
}