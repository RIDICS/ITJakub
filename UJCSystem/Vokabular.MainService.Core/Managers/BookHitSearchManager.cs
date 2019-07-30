using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using Vokabular.DataEntities.Database.ConditionCriteria;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class BookHitSearchManager
    {
        private readonly CriteriaKey[] m_supportedSearchPageCriteria = { CriteriaKey.Fulltext, CriteriaKey.Heading, CriteriaKey.Sentence, CriteriaKey.Term, CriteriaKey.TokenDistance };
        private readonly FulltextStorageProvider m_fulltextStorageProvider;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly BookRepository m_bookRepository;
        private readonly MetadataRepository m_metadataRepository;

        public BookHitSearchManager(FulltextStorageProvider fulltextStorageProvider, AuthorizationManager authorizationManager, BookRepository bookRepository, MetadataRepository metadataRepository)
        {
            m_fulltextStorageProvider = fulltextStorageProvider;
            m_authorizationManager = authorizationManager;
            m_bookRepository = bookRepository;
            m_metadataRepository = metadataRepository;
        }

        public List<PageContract> SearchPage(long projectId, SearchPageRequestContract request)
        {
            m_authorizationManager.AuthorizeBook(projectId);

            var termConditions = new List<SearchCriteriaContract>();
            var fulltextConditions = new List<SearchCriteriaContract>();
            foreach (var searchCriteriaContract in request.ConditionConjunction)
            {
                if (searchCriteriaContract.Key == CriteriaKey.Term)
                {
                    termConditions.Add(searchCriteriaContract);
                }
                else if (m_supportedSearchPageCriteria.Contains(searchCriteriaContract.Key))
                {
                    fulltextConditions.Add(searchCriteriaContract);
                }
                else
                {
                    throw new HttpErrorCodeException($"Not supported criteria key: {searchCriteriaContract.Key}", HttpStatusCode.BadRequest);
                }
            }

            IList<PageResource> pagesByMetadata = null;
            if (termConditions.Count > 0)
            {
                var termConditionCreator = new TermCriteriaPageConditionCreator();
                termConditionCreator.AddCriteria(termConditions);
                termConditionCreator.SetProjectIds(new[] { projectId });

                pagesByMetadata = m_metadataRepository.InvokeUnitOfWork(x => x.GetPagesWithTerms(termConditionCreator));
            }

            IList<PageResource> pagesByFulltext = null;
            if (fulltextConditions.Count > 0)
            {
                var projectIdentification = m_bookRepository.InvokeUnitOfWork(x => x.GetProjectIdentification(projectId));

                var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();
                var fulltextResult = fulltextStorage.SearchPageByCriteria(fulltextConditions, projectIdentification);

                switch (fulltextResult.SearchResultType)
                {
                    case PageSearchResultType.TextId:
                        pagesByFulltext = m_bookRepository.InvokeUnitOfWork(x => x.GetPagesByTextVersionId(fulltextResult.LongList));
                        break;
                    case PageSearchResultType.TextExternalId:
                        pagesByFulltext = m_bookRepository.InvokeUnitOfWork(x => x.GetPagesByTextExternalId(fulltextResult.StringList, projectId));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            IList<PageResource> resultPages;
            if (pagesByMetadata != null && pagesByFulltext != null)
            {
                resultPages = pagesByMetadata.Intersect(pagesByFulltext)
                    .OrderBy(x => x.Position)
                    .ToList();
            }
            else if (pagesByFulltext != null)
            {
                resultPages = pagesByFulltext;
            }
            else if (pagesByMetadata != null)
            {
                resultPages = pagesByMetadata;
            }
            else
            {
                throw new ArgumentException("No supported search criteria was specified");
            }

            var result = Mapper.Map<List<PageContract>>(resultPages);
            return result;
        }

        private List<SearchCriteriaContract> ExtractFulltextConditions(IList<SearchCriteriaContract> searchCriteriaList)
        {
            var fulltextConditions = new List<SearchCriteriaContract>();
            foreach (var searchCriteriaContract in searchCriteriaList)
            {
                if (m_supportedSearchPageCriteria.Contains(searchCriteriaContract.Key))
                {
                    fulltextConditions.Add(searchCriteriaContract);
                }
                else
                {
                    throw new HttpErrorCodeException($"Not supported criteria key: {searchCriteriaContract.Key}", HttpStatusCode.BadRequest);
                }
            }
            return fulltextConditions;
        }

        public List<PageResultContextContract> SearchHitsWithPageContext(long projectId, SearchHitsRequestContract request)
        {
            m_authorizationManager.AuthorizeBook(projectId);

            var fulltextConditions = ExtractFulltextConditions(request.ConditionConjunction);
            if (fulltextConditions.Count == 0)
            {
                throw new ArgumentException("No supported search criteria was specified");
            }

            var projectIdentification = m_bookRepository.InvokeUnitOfWork(x => x.GetProjectIdentification(projectId));

            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();
            var start = PagingHelper.GetStart(request.Start);
            var count = PagingHelper.GetCount(request.Count);
            var fulltextResult = fulltextStorage.SearchHitsWithPageContext(start, count, request.ContextLength, fulltextConditions, projectIdentification);

            switch (fulltextResult.SearchResultType)
            {
                case PageSearchResultType.TextId:
                    return ConvertSearchResultsByStandardId(fulltextResult.ResultList);
                case PageSearchResultType.TextExternalId:
                    return ConvertSearchResultsByExternalId(projectId, fulltextResult.ResultList);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public long SearchHitsResultCount(long projectId, SearchHitsRequestContract request)
        {
            m_authorizationManager.AuthorizeBook(projectId);

            var fulltextConditions = ExtractFulltextConditions(request.ConditionConjunction);
            if (fulltextConditions.Count == 0)
            {
                throw new ArgumentException("No supported search criteria was specified");
            }

            var projectIdentification = m_bookRepository.InvokeUnitOfWork(x => x.GetProjectIdentification(projectId));

            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();
            var fulltextResultCount = fulltextStorage.SearchHitsResultCount(fulltextConditions, projectIdentification);

            return fulltextResultCount;
        }

        private List<PageResultContextContract> ConvertSearchResultsByExternalId(long projectId, List<PageResultContextData> fulltextResultList)
        {
            var orderedPageResourceList = m_bookRepository.InvokeUnitOfWork(repository =>
            {
                var result = new List<PageResource>();
                foreach (var pageResultContextData in fulltextResultList)
                {
                    var page = repository.GetPagesByTextExternalId(new[] { pageResultContextData.StringId }, projectId);
                    result.Add(page.First());
                }

                return result;
            });

            var resultList = new List<PageResultContextContract>();

            for (var i = 0; i < fulltextResultList.Count; i++)
            {
                var pageResultContext = fulltextResultList[i];
                var dbPage = orderedPageResourceList[i];

                var item = new PageResultContextContract
                {
                    ContextStructure = pageResultContext.ContextStructure,
                    PageId = dbPage.Id,
                    PageName = dbPage.Name,
                };
                resultList.Add(item);
            }

            return resultList;
        }

        private List<PageResultContextContract> ConvertSearchResultsByStandardId(List<PageResultContextData> fulltextResultList)
        {
            throw new NotImplementedException();
        }
    }
}
