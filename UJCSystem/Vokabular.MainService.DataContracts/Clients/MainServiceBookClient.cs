using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Results;
using Vokabular.Shared;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceBookClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private readonly MainServiceRestClient m_client;

        public MainServiceBookClient(MainServiceRestClient client)
        {
            m_client = client;
        }

        #region Book

        public List<PageContract> SearchPage(long projectId, SearchPageRequestContract request)
        {
            try
            {
                var result = m_client.Post<List<PageContract>>($"book/{projectId}/page/search", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<PageResultContextContract> SearchHitsWithPageContext(long projectId, SearchHitsRequestContract request)
        {
            try
            {
                var result = m_client.Post<List<PageResultContextContract>>($"book/{projectId}/hit/search", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long SearchHitsResultCount(long projectId, SearchHitsRequestContract request)
        {
            try
            {
                var result = m_client.Post<long>($"book/{projectId}/hit/search-count", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public BookContract GetBookInfo(long projectId)
        {
            try
            {
                var result = m_client.Get<BookContract>($"book/{projectId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public BookContract GetBookInfoByExternalId(string externalId, ProjectTypeContract projectType)
        {
            try
            {
                var url = UrlQueryBuilder.Create("book/info")
                    .AddParameter("externalId", externalId)
                    .AddParameter("projectType", projectType)
                    .ToQuery();
                var result = m_client.Get<BookContract>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public SearchResultDetailContract GetBookDetail(long projectId)
        {
            try
            {
                var result = m_client.Get<SearchResultDetailContract>($"book/{projectId}/detail");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<PageContract> GetBookPageList(long projectId)
        {
            try
            {
                var result = m_client.Get<List<PageContract>>($"book/{projectId}/page");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<ChapterHierarchyContract> GetBookChapterList(long projectId)
        {
            try
            {
                var result = m_client.Get<List<ChapterHierarchyContract>>($"book/{projectId}/chapter");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public bool HasBookAnyImage(long projectId)
        {
            try
            {
                m_client.Head($"book/{projectId}/image");
                return true;
            }
            catch (HttpRequestException e)
            {
                var statusException = e as HttpErrorCodeException;
                if (statusException?.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }

                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public bool HasBookAnyText(long projectId)
        {
            try
            {
                m_client.Head($"book/{projectId}/text");
                return true;
            }
            catch (HttpRequestException e)
            {
                var statusException = e as HttpErrorCodeException;
                if (statusException?.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }

                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public bool HasPageImage(long pageId)
        {
            try
            {
                m_client.Head($"book/page/{pageId}/image");
                return true;
            }
            catch (HttpRequestException e)
            {
                var statusException = e as HttpErrorCodeException;
                if (statusException?.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }

                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public string GetPageText(long pageId, TextFormatEnumContract format)
        {
            try
            {
                var result = m_client.GetString($"book/page/{pageId}/text?format={format}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public string GetHeadwordText(long headwordId, TextFormatEnumContract format)
        {
            try
            {
                var result = m_client.GetString($"book/headword/{headwordId}/text?format={format}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public FileResultData GetPageImage(long pageId)
        {
            try
            {
                var result = m_client.GetStream($"book/page/{pageId}/image");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public FileResultData GetAudio(long resourceId)
        {
            try
            {
                var result = m_client.GetStream($"book/audio/{resourceId}/data");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<TermContract> GetPageTermList(long pageId)
        {
            try
            {
                var result = m_client.Get<List<TermContract>>($"book/page/{pageId}/term");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public string GetPageTextFromSearch(long pageId, TextFormatEnumContract format,
            SearchPageRequestContract request)
        {
            try
            {
                var result = m_client.PostReturnString($"book/page/{pageId}/text/search?format={format}", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public string GetHeadwordTextFromSearch(long headwordId, TextFormatEnumContract format,
            SearchPageRequestContract request)
        {
            try
            {
                var result = m_client.PostReturnString($"book/headword/{headwordId}/text/search?format={format}", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public string GetEditionNoteText(long projectId, TextFormatEnumContract format)
        {
            try
            {
                var result = m_client.GetString($"book/{projectId}/edition-note/text?format={format}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
        
        public List<BookTypeContract> GetBookTypeList()
        {
            try
            {
                var result = m_client.Get<List<BookTypeContract>>("book/type");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        [Obsolete("This method will be replaced by paged variant")]
        public List<BookWithCategoriesContract> GetBooksByType(BookTypeEnumContract bookTypeEnum)
        {
            try
            {
                var result = m_client.Get<List<BookWithCategoriesContract>>($"book/type/{bookTypeEnum}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<SearchResultContract> SearchBook(AdvancedSearchRequestContract request, ProjectTypeContract projectType)
        {
            try
            {
                var result = m_client.Post<List<SearchResultContract>>($"book/search?projectType={projectType}", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long SearchBookCount(AdvancedSearchRequestContract request, ProjectTypeContract projectType)
        {
            try
            {
                var result = m_client.Post<long>($"book/search-count?projectType={projectType}", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Headword

        public long SearchHeadwordCount(HeadwordSearchRequestContract request, ProjectTypeContract projectType)
        {
            try
            {
                var result = m_client.Post<long>($"headword/search-count?projectType={projectType}", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long SearchHeadwordRowNumber(HeadwordRowNumberSearchRequestContract request, ProjectTypeContract projectType)
        {
            try
            {
                var result = m_client.Post<long>($"headword/search-row-number?projectType={projectType}", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }


        public List<HeadwordContract> SearchHeadword(HeadwordSearchRequestContract request, ProjectTypeContract projectType)
        {
            try
            {
                var result = m_client.Post<List<HeadwordContract>>($"headword/search?projectType={projectType}", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<string> GetHeadwordAutocomplete(string query, ProjectTypeContract projectType, BookTypeEnumContract? bookType = null,
            IList<int> selectedCategoryIds = null, IList<long> selectedProjectIds = null)
        {
            try
            {
                var url = UrlQueryBuilder.Create("headword/autocomplete")
                    .AddParameter("query", query)
                    .AddParameter("projectType", projectType)
                    .AddParameter("bookType", bookType)
                    .AddParameterList("selectedCategoryIds", selectedCategoryIds)
                    .AddParameterList("selectedProjectIds", selectedProjectIds)
                    .ToQuery();

                var result = m_client.Get<List<string>>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Corpus

        public List<CorpusSearchResultContract> SearchCorpus(CorpusSearchRequestContract request, ProjectTypeContract projectType)
        {
            try
            {
                var result = m_client.Post<List<CorpusSearchResultContract>>($"corpus/search?projectType={projectType}", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long SearchCorpusCount(CorpusSearchRequestContract request, ProjectTypeContract projectType)
        {
            try
            {
                var result = m_client.Post<long>($"corpus/search-count?projectType={projectType}", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region BookPagedCorpus

        public CorpusSearchSnapshotsResultContract SearchCorpusGetSnapshotList(CorpusSearchRequestContract request, ProjectTypeContract projectType)
        {
            try
            {
                var result = m_client.Post<CorpusSearchSnapshotsResultContract>($"bookpagedcorpus/search?projectType={projectType}", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<CorpusSearchResultContract> SearchCorpusInSnapshot(long snapshotId, CorpusSearchRequestContract request)
        {
            try
            {
                var result = m_client.Post<List<CorpusSearchResultContract>>($"bookpagedcorpus/snapshot/{snapshotId}/search", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long SearchCorpusTotalResultCount(SearchRequestContractBase request, ProjectTypeContract projectType)
        {
            try
            {
                var result = m_client.Post<long>($"bookpagedcorpus/search-count?projectType={projectType}", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region AudioBook

        public AudioBookSearchResultContract GetAudioBookDetail(long projectId)
        {
            try
            {
                var result = m_client.Get<AudioBookSearchResultContract>($"audiobook/{projectId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public List<AudioBookSearchResultContract> SearchAudioBook(SearchRequestContract request, ProjectTypeContract projectType)
        {
            try
            {
                var result = m_client.Post<List<AudioBookSearchResultContract>>($"audiobook/search?projectType={projectType}", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion
    }
}
