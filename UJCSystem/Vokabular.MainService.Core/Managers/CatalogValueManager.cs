using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.CatalogValues;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Results;

namespace Vokabular.MainService.Core.Managers
{
    public class CatalogValueManager
    {
        private readonly CatalogValueRepository m_catalogValueRepository;

        public CatalogValueManager(CatalogValueRepository catalogValueRepository)
        {
            m_catalogValueRepository = catalogValueRepository;
        }

        public int CreateLiteraryGenre(string name)
        {
            return new CreateOrUpdateLiteraryGenreWork(m_catalogValueRepository, null, name).Execute();
        }

        public int CreateLiteraryKind(string name)
        {
            return new CreateOrUpdateLiteraryKindWork(m_catalogValueRepository, null, name).Execute();
        }
        
        public int CreateLiteraryOriginal(string name)
        {
            return new CreateOrUpdateLiteraryOriginalWork(m_catalogValueRepository, null, name).Execute();
        }

        public int CreateKeyword(string name)
        {
            return new CreateOrUpdateKeywordWork(m_catalogValueRepository, null, name).Execute();
        }

        public int CreateResponsibleType(ResponsibleTypeContract responsibleTypeData)
        {
            return new CreateOrUpdateResponsibleTypeWork(m_catalogValueRepository, null, responsibleTypeData).Execute();
        }

        public int CreateTermCategory(TermCategoryContract data)
        {
            return new CreateOrUpdateTermCategoryWork(m_catalogValueRepository, null, data).Execute();
        }

        public List<LiteraryGenreContract> GetLiteraryGenreList()
        {
            var result = m_catalogValueRepository.InvokeUnitOfWork(x => x.GetLiteraryGenreList());
            return Mapper.Map<List<LiteraryGenreContract>>(result);
        }

        public List<LiteraryKindContract> GetLiteraryKindList()
        {
            var result = m_catalogValueRepository.InvokeUnitOfWork(x => x.GetLiteraryKindList());
            return Mapper.Map<List<LiteraryKindContract>>(result);
        }

        public List<LiteraryOriginalContract> GetLiteraryOriginalList()
        {
            var result = m_catalogValueRepository.InvokeUnitOfWork(x => x.GetLiteraryOriginalList());
            return Mapper.Map<List<LiteraryOriginalContract>>(result);
        }

        public PagedResultList<KeywordContract> GetKeywordList(int? start, int? count)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);
            var dbResult = m_catalogValueRepository.InvokeUnitOfWork(x => x.GetKeywordList(startValue, countValue));
            return new PagedResultList<KeywordContract>
            {
                List = Mapper.Map<List<KeywordContract>>(dbResult.List),
                TotalCount = dbResult.Count,
            };
        }

        public List<ResponsibleTypeContract> GetResponsibleTypeList()
        {
            var resultList = m_catalogValueRepository.InvokeUnitOfWork(x => x.GetResponsibleTypeList());
            return Mapper.Map<List<ResponsibleTypeContract>>(resultList);
        }
        
        public List<TermCategoryContract> GetTermCategoryList()
        {
            var resultList = m_catalogValueRepository.InvokeUnitOfWork(x => x.GetTermCategoryList());
            return Mapper.Map<List<TermCategoryContract>>(resultList);
        }

        public void UpdateLiteraryGenre(int literaryGenreId, LiteraryGenreContract data)
        {
            new CreateOrUpdateLiteraryGenreWork(m_catalogValueRepository, literaryGenreId, data.Name).Execute();
        }

        public void UpdateLiteraryKind(int literaryKindId, LiteraryKindContract data)
        {
            new CreateOrUpdateLiteraryKindWork(m_catalogValueRepository, literaryKindId, data.Name).Execute();
        }

        public void UpdateLiteraryOriginal(int literaryOriginalId, LiteraryOriginalContract data)
        {
            new CreateOrUpdateLiteraryOriginalWork(m_catalogValueRepository, literaryOriginalId, data.Name).Execute();
        }

        public void UpdateKeyword(int keywordId, KeywordContract data)
        {
            new CreateOrUpdateKeywordWork(m_catalogValueRepository, keywordId, data.Name).Execute();
        }

        public void UpdateResponsibleType(int responsibleTypeId, ResponsibleTypeContract data)
        {
            new CreateOrUpdateResponsibleTypeWork(m_catalogValueRepository, responsibleTypeId, data).Execute();
        }

        public void UpdateTermCategory(int termCategoryId, TermCategoryContract data)
        {
            new CreateOrUpdateTermCategoryWork(m_catalogValueRepository, termCategoryId, data).Execute();
        }

        public void DeleteLiteraryGenre(int literaryGenreId)
        {
            new DeleteCatalogValueWork<LiteraryGenre>(m_catalogValueRepository, literaryGenreId).Execute();
        }

        public void DeleteLiteraryKind(int literaryKindId)
        {
            new DeleteCatalogValueWork<LiteraryKind>(m_catalogValueRepository, literaryKindId).Execute();
        }

        public void DeleteLiteraryOriginal(int literaryOriginalId)
        {
            new DeleteCatalogValueWork<LiteraryOriginal>(m_catalogValueRepository, literaryOriginalId).Execute();
        }

        public void DeleteKeyword(int keywordId)
        {
            new DeleteCatalogValueWork<Keyword>(m_catalogValueRepository, keywordId).Execute();
        }

        public void DeleteResponsibleType(int responsibleTypeId)
        {
            new DeleteCatalogValueWork<ResponsibleType>(m_catalogValueRepository, responsibleTypeId).Execute();
        }

        public void DeleteTermCategory(int termCategoryId)
        {
            new DeleteCatalogValueWork<TermCategory>(m_catalogValueRepository, termCategoryId).Execute();
        }

        public LiteraryGenreContract GetLiteraryGenre(int literaryGenreId)
        {
            var result = m_catalogValueRepository.InvokeUnitOfWork(x => x.FindById<LiteraryGenre>(literaryGenreId));
            return Mapper.Map<LiteraryGenreContract>(result);
        }
        
        public LiteraryKindContract GetLiteraryKind(int literaryKindId)
        {
            var result = m_catalogValueRepository.InvokeUnitOfWork(x => x.FindById<LiteraryKind>(literaryKindId));
            return Mapper.Map<LiteraryKindContract>(result);
        }
        
        public LiteraryOriginalContract GetLiteraryOriginal(int literaryOriginalId)
        {
            var result = m_catalogValueRepository.InvokeUnitOfWork(x => x.FindById<LiteraryOriginal>(literaryOriginalId));
            return Mapper.Map<LiteraryOriginalContract>(result);
        }

        public KeywordContract GetKeyword(int keywordId)
        {
            var result = m_catalogValueRepository.InvokeUnitOfWork(x => x.FindById<Keyword>(keywordId));
            return Mapper.Map<KeywordContract>(result);
        }
        
        public ResponsibleTypeContract GetResponsibleType(int responsibleTypeId)
        {
            var result = m_catalogValueRepository.InvokeUnitOfWork(x => x.FindById<ResponsibleType>(responsibleTypeId));
            return Mapper.Map<ResponsibleTypeContract>(result);
        }
        
        public TermCategoryContract GetTermCategory(int termCategoryId)
        {
            var result = m_catalogValueRepository.InvokeUnitOfWork(x => x.FindById<TermCategory>(termCategoryId));
            return Mapper.Map<TermCategoryContract>(result);
        }

        public List<TermCategoryDetailContract> GetTermCategoriesWithTerms()
        {
            var result = m_catalogValueRepository.InvokeUnitOfWork(x => x.GetTermCategoriesWithTerms());
            return Mapper.Map<List<TermCategoryDetailContract>>(result);
        }

        public List<KeywordContract> GetKeywordAutocomplete(string query, int? count)
        {
            var countValue = PagingHelper.GetAutocompleteCount(count);
            var result = m_catalogValueRepository.InvokeUnitOfWork(x => x.GetKeywordAutocomplete(query, countValue));
            return Mapper.Map<List<KeywordContract>>(result);
        }
    }
}
