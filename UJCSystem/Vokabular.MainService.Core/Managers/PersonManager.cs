using System;
using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Person;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers
{
    public class PersonManager
    {
        private const int DefaultStart = 0;
        private const int DefaultCount = 20;
        private const int MaxResultCount = 200;
        private readonly PersonRepository m_personRepository;

        public PersonManager(PersonRepository personRepository)
        {
            m_personRepository = personRepository;
        }

        private int GetStart(int? start)
        {
            return start ?? DefaultStart;
        }

        private int GetCount(int? count)
        {
            return count != null ? Math.Min(count.Value, MaxResultCount) : DefaultCount;
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
            var result = Mapper.Map<OriginalAuthorContract>(dbResult);
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
            return Mapper.Map<ResponsiblePersonContract>(responsiblePerson);
        }

        #endregion


        public List<OriginalAuthorContract> GetAuthorAutocomplete(string query, BookTypeEnumContract? bookType)
        {
            if (query == null)
                query = string.Empty;

            var bookTypeEnum = Mapper.Map<BookTypeEnum?>(bookType);
            var result = m_personRepository.InvokeUnitOfWork(x => x.GetAuthorAutocomplete(query, bookTypeEnum, DefaultValues.AutocompleteMaxCount));
            return Mapper.Map<List<OriginalAuthorContract>>(result);
        }

        public List<ResponsiblePersonContract> GetResponsiblePersonAutocomplete(string query)
        {
            if (query == null)
                query = string.Empty;

            var result = m_personRepository.InvokeUnitOfWork(x => x.GetResponsiblePersonAutocomplete(query, DefaultValues.AutocompleteMaxCount));
            return Mapper.Map<List<ResponsiblePersonContract>>(result);
        }

        public PagedResultList<ResponsiblePersonContract> GetResponsiblePersonList(int? start, int? count)
        {
            var dbResult = m_personRepository.InvokeUnitOfWork(x => x.GetResponsiblePersonList(GetStart(start), GetCount(count)));
            var resultList = Mapper.Map<List<ResponsiblePersonContract>>(dbResult.List);

            return new PagedResultList<ResponsiblePersonContract>
            {
                List = resultList,
                TotalCount = dbResult.Count
            };
        }

        public PagedResultList<OriginalAuthorContract> GetOriginalAuthorList(int? start, int? count)
        {
            var dbResult = m_personRepository.InvokeUnitOfWork(x => x.GetOriginalAuthorList(GetStart(start), GetCount(count)));
            var resultList = Mapper.Map<List<OriginalAuthorContract>>(dbResult.List);

            return new PagedResultList<OriginalAuthorContract>
            {
                List = resultList,
                TotalCount = dbResult.Count
            };
        }
    }
}