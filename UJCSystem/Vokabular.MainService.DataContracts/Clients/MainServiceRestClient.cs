using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.CardFile;
using Vokabular.MainService.DataContracts.Contracts.Favorite;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using Vokabular.MainService.DataContracts.Contracts.OaiPmh;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Extensions;
using Vokabular.RestClient.Results;
using Vokabular.Shared;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataContracts.Types.Favorite;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceRestClient : FullRestClientBase
    {
        private readonly IMainServiceAuthTokenProvider m_tokenProvider;
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private const string AuthenticationScheme = "Bearer";

        public MainServiceRestClient(IMainServiceUriProvider uriProvider, IMainServiceAuthTokenProvider tokenProvider) : base(uriProvider.MainServiceUri)
        {
            m_tokenProvider = tokenProvider;
        }

        protected override void FillRequestMessage(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(AuthenticationScheme, m_tokenProvider.AuthToken);
        }

        protected override void ProcessResponse(HttpResponseMessage response)
        {
        }

        public List<string> GetPublisherAutoComplete(string query)
        {
            try
            {
                var publishers =  Get<List<string>>($"metadata/publisher/autocomplete?query={query}");
                return publishers;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}",  GetCurrentMethod(), e);

                throw;
            }
        }

        #region Configure project relationships

      

        #endregion


        public void UploadResource(string sessionId, Stream data, string fileName)
        {
            try
            {
                var uriPath = $"session/{sessionId}/resource";
                PostStreamAsForm<object>(uriPath, data, fileName);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void ProcessSessionAsImport(string sessionId, NewBookImportContract request)
        {
            try
            {
                HttpClient.Timeout = new TimeSpan(0, 10, 0); // Import is long running operation
                Post<object>($"session/{sessionId}",request);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteResource(long resourceId)
        {
            try
            {
                Delete($"resource/{resourceId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long DuplicateResource(long resourceId)
        {
            try
            {
                var newResourceId = Post<long>($"resource/{resourceId}/duplicate", null);
                return newResourceId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<ResourceVersionContract> GetResourceVersionHistory(long resourceId)
        {
            try
            {
                var result = Get<List<ResourceVersionContract>>($"resource/{resourceId}/version");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public ResourceMetadataContract GetResourceMetadata(long resourceId)
        {
            try
            {
                var result = Get<ResourceMetadataContract>($"resource/{resourceId}/metadata");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long ProcessUploadedResourceVersion(long resourceId, NewResourceContract resourceInfo)
        {
            try
            {
                var resourceVersionId = Post<long>($"resource/{resourceId}/version", resourceInfo);
                return resourceVersionId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void RenameResource(long resourceId, ResourceContract resource)
        {
            try
            {
                Put<object>($"resource/{resourceId}", resource);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #region Category

        public int CreateCategory(CategoryContract category)
        {
            try
            {
                var resultId = Post<int>("category", category);
                return resultId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public object UpdateCategory(int categoryId, CategoryContract category)
        {
            try
            {
                var resultId = Put<object>($"category/{categoryId}", category);
                return resultId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteCategory(int categoryId)
        {
            try
            {
                Delete($"category/{categoryId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<CategoryContract> GetCategoryList()
        {
            try
            {
                var result = Get<List<CategoryContract>>("category");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Genre

        public int CreateLiteraryGenre(LiteraryGenreContract literaryGenre)
        {
            try
            {
                var newId = Post<int>("literarygenre", literaryGenre);
                return newId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateLiteraryGenre(int literaryGenreId, LiteraryGenreContract data)
        {
            try
            {
                Put<HttpStatusCode>($"literarygenre/{literaryGenreId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteLiteraryGenre(int literaryGenreId)
        {
            try
            {
                Delete($"literarygenre/{literaryGenreId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<LiteraryGenreContract> GetLiteraryGenreList()
        {
            try
            {
                var result = Get<List<LiteraryGenreContract>>("literarygenre");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Literary Kind

        public List<LiteraryKindContract> GetLiteraryKindList()
        {
            try
            {
                var result = Get<List<LiteraryKindContract>>("literarykind");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateLiteraryKind(LiteraryKindContract literaryKind)
        {
            try
            {
                var newId = Post<int>("literarykind", literaryKind);
                return newId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateLiteraryKind(int literaryKindId, LiteraryKindContract data)
        {
            try
            {
                Put<HttpStatusCode>($"literarykind/{literaryKindId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteLiteraryKind(int literaryKindId)
        {
            try
            {
                Delete($"literarykind/{literaryKindId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Responsible person

        public PagedResultList<ProjectDetailContract> GetProjectsByResponsiblePerson(int responsiblePersonId, int? start, int? count)
        {
            try
            {
                var result = GetPagedList<ProjectDetailContract>(
                    $"responsibleperson/{responsiblePersonId}/project?start={start}&count={count}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateResponsiblePerson(ResponsiblePersonContract responsiblePerson)
        {
            try
            {
                var newId = Post<int>("responsibleperson", responsiblePerson);
                return newId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<ResponsiblePersonContract> GetResponsiblePersonList(int start, int count)
        {
            try
            {
                var result = GetPagedList<ResponsiblePersonContract>($"responsibleperson?start={start}&count={count}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateResponsiblePerson(int responsiblePersonId, ResponsiblePersonContract data)
        {
            try
            {
                Put<HttpStatusCode>($"responsibleperson/{responsiblePersonId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteResponsiblePerson(int responsiblePersonId)
        {
            try
            {
                Delete($"responsibleperson/{responsiblePersonId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Responsible person type

        public List<ResponsibleTypeContract> GetResponsibleTypeList()
        {
            try
            {
                var result = Get<List<ResponsibleTypeContract>>("responsibleperson/type");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateResponsibleType(ResponsibleTypeContract responsibleType)
        {
            try
            {
                var resultId = Post<int>("responsibleperson/type", responsibleType);
                return resultId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteResponsibleType(int responsibleTypeId)
        {
            try
            {
                Delete($"responsibleperson/type/{responsibleTypeId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateResponsibleType(int responsibleTypeId, ResponsibleTypeContract data)
        {
            try
            {
                Put<HttpStatusCode>($"responsibleperson/type/{responsibleTypeId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Literary Original

        public List<LiteraryOriginalContract> GetLiteraryOriginalList()
        {
            try
            {
                var result = Get<List<LiteraryOriginalContract>>("literaryoriginal");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateLiteraryOriginal(LiteraryOriginalContract literaryOriginal)
        {
            try
            {
                var newId = Post<int>("literaryoriginal", literaryOriginal);
                return newId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateLiteraryOriginal(int literaryOriginalId, LiteraryOriginalContract data)
        {
            try
            {
                Put<HttpStatusCode>($"literaryoriginal/{literaryOriginalId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteLiteraryOriginal(int literaryOriginalId)
        {
            try
            {
                Delete($"literaryoriginal/{literaryOriginalId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Original author

        public PagedResultList<ProjectDetailContract> GetProjectsByAuthor(int authorId, int? start, int? count)
        {
            try
            {
                var result = GetPagedList<ProjectDetailContract>($"author/{authorId}/project?start={start}&count={count}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<OriginalAuthorContract> GetOriginalAuthorList(int start, int count)
        {
            try
            {
                var result = GetPagedList<OriginalAuthorContract>($"author?start={start}&count={count}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateOriginalAuthor(OriginalAuthorContract author)
        {
            try
            {
                var newId = Post<int>("author", author);
                return newId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateOriginalAuthor(int authorId, OriginalAuthorContract data)
        {
            try
            {
                Put<HttpStatusCode>($"author/{authorId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteOriginalAuthor(int authorId)
        {
            try
            {
                Delete($"author/{authorId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Keyword

        public List<KeywordContract> GetKeywordAutocomplete(string query, int? count)
        {
            try
            {
                var result = Get<List<KeywordContract>>($"keyword/autocomplete?query={query}&count={count}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<KeywordContract> GetKeywordList(int? start, int? count)
        {
            try
            {
                var result = GetPagedList<KeywordContract>($"keyword?start={start}&count={count}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateKeyword(KeywordContract keyword)
        {
            try
            {
                var newId = Post<int>("keyword", keyword);
                return newId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateKeyword(int keywordId, KeywordContract data)
        {
            try
            {
                Put<HttpStatusCode>($"keyword/{keywordId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteKeyword(int keywordId)
        {
            try
            {
                Delete($"keyword/{keywordId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        public List<OriginalAuthorContract> GetOriginalAuthorAutocomplete(string query,
            BookTypeEnumContract? bookType = null)
        {
            try
            {
                var url = UrlQueryBuilder.Create("author/autocomplete")
                    .AddParameter("query", query)
                    .AddParameter("bookType", bookType)
                    .ToQuery();

                var result = Get<List<OriginalAuthorContract>>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<string> GetTitleAutocomplete(string query, BookTypeEnumContract? bookType = null,
            IList<int> selectedCategoryIds = null, IList<long> selectedProjectIds = null)
        {
            try
            {
                var url = UrlQueryBuilder.Create("metadata/title/autocomplete")
                    .AddParameter("query", query)
                    .AddParameter("bookType", bookType)
                    .AddParameterList("selectedCategoryIds", selectedCategoryIds)
                    .AddParameterList("selectedProjectIds", selectedProjectIds)
                    .ToQuery();

                var result = Get<List<string>>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<string> GetHeadwordAutocomplete(string query, BookTypeEnumContract? bookType = null,
            IList<int> selectedCategoryIds = null, IList<long> selectedProjectIds = null)
        {
            try
            {
                var url = UrlQueryBuilder.Create("headword/autocomplete")
                    .AddParameter("query", query)
                    .AddParameter("bookType", bookType)
                    .AddParameterList("selectedCategoryIds", selectedCategoryIds)
                    .AddParameterList("selectedProjectIds", selectedProjectIds)
                    .ToQuery();

                var result = Get<List<string>>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<ResponsiblePersonContract> GetResponsiblePersonAutocomplete(string query)
        {
            try
            {
                var result =
                    Get<List<ResponsiblePersonContract>>(
                        "responsibleperson/autocomplete".AddQueryString("query", query));
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<BookWithCategoriesContract> GetBooksByType(BookTypeEnumContract bookTypeEnum)
        {
            try
            {
                var result = Get<List<BookWithCategoriesContract>>($"book/type/{bookTypeEnum}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #region Searching

        public List<SearchResultContract> SearchBook(SearchRequestContract request)
        {
            try
            {
                var result = Post<List<SearchResultContract>>("book/search", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<HeadwordContract> SearchHeadword(HeadwordSearchRequestContract request)
        {
            try
            {
                var result = Post<List<HeadwordContract>>("headword/search", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<AudioBookSearchResultContract> SearchAudioBook(SearchRequestContract request)
        {
            try
            {
                var result = Post<List<AudioBookSearchResultContract>>("audiobook/search", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long SearchBookCount(SearchRequestContract request)
        {
            try
            {
                var result = Post<long>("book/search-count", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long SearchHeadwordCount(HeadwordSearchRequestContract request)
        {
            try
            {
                var result = Post<long>("headword/search-count", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long SearchHeadwordRowNumber(HeadwordRowNumberSearchRequestContract request)
        {
            try
            {
                var result = Post<long>("headword/search-row-number", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<PageContract> SearchPage(long projectId, SearchPageRequestContract request)
        {
            try
            {
                var result = Post<List<PageContract>>($"book/{projectId}/page/search", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<CorpusSearchResultContract> SearchCorpus(CorpusSearchRequestContract request)
        {
            try
            {
                var result = Post<List<CorpusSearchResultContract>>("corpus/search", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long SearchCorpusCount(CorpusSearchRequestContract request)
        {
            try
            {
                var result = Post<long>("corpus/search-count", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public CorpusSearchSnapshotsResultContract SearchCorpusGetSnapshotList(CorpusSearchRequestContract request)
        {
            try
            {
                var result = Post<CorpusSearchSnapshotsResultContract>("bookpagedcorpus/search", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<CorpusSearchResultContract> SearchCorpusInSnapshot(long snapshotId, CorpusSearchRequestContract request)
        {
            try
            {
                var result = Post<List<CorpusSearchResultContract>>($"bookpagedcorpus/snapshot/{snapshotId}/search", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long SearchCorpusTotalResultCount(SearchRequestContractBase request)
        {
            try
            {
                var result = Post<long>("bookpagedcorpus/search-count", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<PageResultContextContract> SearchHitsWithPageContext(long projectId, SearchHitsRequestContract request)
        {
            try
            {
                var result = Post<List<PageResultContextContract>>($"book/{projectId}/hit/search", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long SearchHitsResultCount(long projectId, SearchHitsRequestContract request)
        {
            try
            {
                var result = Post<long>($"book/{projectId}/hit/search-count", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        public BookContract GetBookInfo(long projectId)
        {
            try
            {
                var result = Get<BookContract>($"book/{projectId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public BookContract GetBookInfoByExternalId(string externalId)
        {
            try
            {
                var result = Get<BookContract>("book/info".AddQueryString("externalId", externalId));
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public SearchResultDetailContract GetBookDetail(long projectId)
        {
            try
            {
                var result = Get<SearchResultDetailContract>($"book/{projectId}/detail");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public AudioBookSearchResultContract GetAudioBookDetail(long projectId)
        {
            try
            {
                var result = Get<AudioBookSearchResultContract>($"audiobook/{projectId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long SetAllPageList(string[] pageList)
        {
            try
            {
                //TODO add logic for saving page list after editing
                throw new NotImplementedException();
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateNewTextResourceVersion(long textId, TextContract request)
        {
            try
            {
                var result = Post<long>($"text/{textId}", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<PageContract> GetBookPageList(long projectId)
        {
            try
            {
                var result = Get<List<PageContract>>($"book/{projectId}/page");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<ChapterHierarchyContract> GetBookChapterList(long projectId)
        {
            try
            {
                var result = Get<List<ChapterHierarchyContract>>($"book/{projectId}/chapter");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public bool HasBookAnyImage(long projectId)
        {
            try
            {
                Head($"book/{projectId}/image");
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
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public bool HasBookAnyText(long projectId)
        {
            try
            {
                Head($"book/{projectId}/text");
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
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public string GetPageText(long pageId, TextFormatEnumContract format)
        {
            try
            {
                var result = GetString($"book/page/{pageId}/text?format={format}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public string GetHeadwordText(long headwordId, TextFormatEnumContract format)
        {
            try
            {
                var result = GetString($"book/headword/{headwordId}/text?format={format}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public FileResultData GetPageImage(long pageId)
        {
            try
            {
                var result = GetStream($"book/page/{pageId}/image");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public FileResultData GetAudio(long resourceId)
        {
            try
            {
                var result = GetStream($"book/audio/{resourceId}/data");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<TermContract> GetPageTermList(long pageId)
        {
            try
            {
                var result = Get<List<TermContract>>($"book/page/{pageId}/term");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public string GetPageTextFromSearch(long pageId, TextFormatEnumContract format,
            SearchPageRequestContract request)
        {
            try
            {
                var result = PostReturnString($"book/page/{pageId}/text/search?format={format}", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public string GetHeadwordTextFromSearch(long headwordId, TextFormatEnumContract format,
            SearchPageRequestContract request)
        {
            try
            {
                var result = PostReturnString($"book/headword/{headwordId}/text/search?format={format}", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public string GetEditionNote(long projectId, TextFormatEnumContract format)
        {
            try
            {
                var result = GetString($"book/{projectId}/edition-note?format={format}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<TermCategoryDetailContract> GetTermCategoriesWithTerms()
        {
            try
            {
                var result = Get<List<TermCategoryDetailContract>>("term/category/detail");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }


        #region Favorite items

        public List<FavoriteLabelContract> GetFavoriteLabelList(int? count = null)
        {
            try
            {
                var result = Get<List<FavoriteLabelContract>>(UrlQueryBuilder.Create("favorite/label").AddParameter("count", count)
                    .ToQuery());
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateFavoriteLabel(FavoriteLabelContractBase data)
        {
            try
            {
                var result = Post<long>("favorite/label", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateFavoriteLabel(long favoriteLabelId, FavoriteLabelContractBase data)
        {
            try
            {
                Put<object>($"favorite/label/{favoriteLabelId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteFavoriteLabel(long favoriteLabelId)
        {
            try
            {
                Delete($"favorite/label/{favoriteLabelId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<FavoriteBaseInfoContract> GetFavoriteItems(int start, int count, long? filterByLabelId,
            FavoriteTypeEnumContract? filterByType, string filterByTitle, FavoriteSortEnumContract? sort)
        {
            try
            {
                var url = UrlQueryBuilder.Create("favorite")
                    .AddParameter("start", start)
                    .AddParameter("count", count)
                    .AddParameter("filterByLabelId", filterByLabelId)
                    .AddParameter("filterByType", filterByType)
                    .AddParameter("filterByTitle", filterByTitle)
                    .AddParameter("sort", sort)
                    .ToQuery();

                var result = GetPagedList<FavoriteBaseInfoContract>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<FavoriteQueryContract> GetFavoriteQueries(int start, int count, long? filterByLabelId,
            BookTypeEnumContract? bookType, QueryTypeEnumContract? queryType, string filterByTitle)
        {
            try
            {
                var url = UrlQueryBuilder.Create("favorite/query")
                    .AddParameter("start", start)
                    .AddParameter("count", count)
                    .AddParameter("filterByLabelId", filterByLabelId)
                    .AddParameter("bookType", bookType)
                    .AddParameter("queryType", queryType)
                    .AddParameter("filterByTitle", filterByTitle)
                    .ToQuery();

                var result = GetPagedList<FavoriteQueryContract>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<FavoritePageContract> GetPageBookmarks(long projectId)
        {
            try
            {
                var result = Get<List<FavoritePageContract>>($"favorite/page?projectId={projectId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public FavoriteFullInfoContract GetFavoriteItem(long favoriteId)
        {
            try
            {
                var result = Get<FavoriteFullInfoContract>($"favorite/{favoriteId}/detail");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateFavoriteItem(long favoriteId, UpdateFavoriteContract data)
        {
            try
            {
                Put<object>($"favorite/{favoriteId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteFavoriteItem(long favoriteId)
        {
            try
            {
                Delete($"favorite/{favoriteId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<FavoriteBookGroupedContract> GetFavoriteLabeledBooks(IList<long> projectIds, BookTypeEnumContract? bookType)
        {
            try
            {
                var url = UrlQueryBuilder.Create("favorite/book/grouped")
                    .AddParameterList("projectIds", projectIds)
                    .AddParameter("bookType", bookType)
                    .ToQuery();

                var result = Get<List<FavoriteBookGroupedContract>>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<FavoriteCategoryGroupedContract> GetFavoriteLabeledCategories()
        {
            try
            {
                var result = Get<List<FavoriteCategoryGroupedContract>>("favorite/category/grouped");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<FavoriteLabelWithBooksAndCategories> GetFavoriteLabelsWithBooksAndCategories(BookTypeEnumContract bookType)
        {
            try
            {
                var result = Get<List<FavoriteLabelWithBooksAndCategories>>(
                    $"favorite/label/with-books-and-categories?bookType={bookType.ToString()}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateFavoriteBook(CreateFavoriteProjectContract data)
        {
            try
            {
                var result = Post<long>("favorite/book", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateFavoriteCategory(CreateFavoriteCategoryContract data)
        {
            try
            {
                var result = Post<long>("favorite/category", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateFavoriteQuery(CreateFavoriteQueryContract data)
        {
            try
            {
                var result = Post<long>("favorite/query", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateFavoritePage(CreateFavoritePageContract data)
        {
            try
            {
                var result = Post<long>("favorite/page", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region News

        public PagedResultList<NewsSyndicationItemContract> GetNewsSyndicationItems(int start, int count, NewsTypeEnumContract? itemType)
        {
            try
            {
                var url = UrlQueryBuilder.Create("news")
                    .AddParameter("start", start)
                    .AddParameter("count", count)
                    .AddParameter("itemType", itemType)
                    .ToQuery();
                var result = GetPagedList<NewsSyndicationItemContract>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateNewsSyndicationItem(CreateNewsSyndicationItemContract data)
        {
            try
            {
                var result = Post<long>("news", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Feedback

        public long CreateFeedback(CreateFeedbackContract data)
        {
            try
            {
                var result = Post<long>("feedback", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateAnonymousFeedback(CreateAnonymousFeedbackContract data)
        {
            try
            {
                var result = Post<long>("feedback/anonymous", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateHeadwordFeedback(long resourceVersionId, CreateFeedbackContract data)
        {
            try
            {
                var result = Post<long>($"feedback/headword/version/{resourceVersionId}", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public long CreateAnonymousHeadwordFeedback(long resourceVersionId, CreateAnonymousFeedbackContract data)
        {
            try
            {
                var result = Post<long>($"feedback/headword/version/{resourceVersionId}/anonymous", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<FeedbackContract> GetFeedbackList(int start,
            int count,
            FeedbackSortEnumContract sort,
            SortDirectionEnumContract sortDirection,
            IList<FeedbackCategoryEnumContract> filterCategories)
        {
            try
            {
                var result = GetPagedList<FeedbackContract>("feedback");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteFeedback(long feedbackId)
        {
            try
            {
                Delete($"feedback/{feedbackId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Authentication and users

        public int CreateNewUser(CreateUserContract data)
        {
            try
            {
                //EnsureSecuredClient();
                var result = Post<int>("user", data);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateUserIfNotExist(int userExternalId)
        {
            try
            {
                var result = Post<int>("user/external", userExternalId);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public UserDetailContract GetCurrentUser()
        {
            try
            {
                return Get<UserDetailContract>("user/current");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateCurrentUser(UpdateUserContract data)
        {
            try
            {
                //EnsureSecuredClient();
                Put<object>("user/current", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateCurrentPassword(UpdateUserPasswordContract data)
        {
            try
            {
                //EnsureSecuredClient();
                Put<object>("user/current/password", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateCurrentUserContact(UpdateUserContactContract data)
        {
            try
            {
                Put<object>("user/current/contact", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }
        
        #endregion

        #region Card files

        public List<CardFileContract> GetCardFiles()
        {
            try
            {
                var result = Get<List<CardFileContract>>("cardfile");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<BucketShortContract> GetBuckets(string cardFileId, string headword = null)
        {
            try
            {
                var url = UrlQueryBuilder.Create($"cardfile/{cardFileId}/bucket")
                    .AddParameter("headword", headword)
                    .ToQuery();
                var result = Get<List<BucketShortContract>>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<CardContract> GetCards(string cardFileId, string bucketId)
        {
            try
            {
                var result = Get<List<CardContract>>($"cardfile/{cardFileId}/bucket/{bucketId}/card");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<CardShortContract> GetCardsShort(string cardFileId, string bucketId)
        {
            try
            {
                var result = Get<List<CardShortContract>>($"cardfile/{cardFileId}/bucket/{bucketId}/card/short");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public CardContract GetCard(string cardFileId, string bucketId, string cardId)
        {
            try
            {
                var result = Get<CardContract>($"cardfile/{cardFileId}/bucket/{bucketId}/card/{cardId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public FileResultData GetCardImage(string cardFileId, string bucketId, string cardId, string imageId,
            CardImageSizeEnumContract imageSize)
        {
            try
            {
                var result = GetStream($"cardfile/{cardFileId}/bucket/{bucketId}/card/{cardId}/image/{imageId}?imageSize={imageSize}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Permissions

        #region User

        public UserDetailContract GetUserDetail(int userId)
        {
            try
            {
                var result = Get<UserDetailContract>($"user/{userId}/detail");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<UserDetailContract> GetUserAutocomplete(string query)
        {
            try
            {
                var result = Get<List<UserDetailContract>>("user/autocomplete".AddQueryString("query", query));
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<UserDetailContract> GetUserList(int start, int count, string query)
        {
            try
            {
                var url = "user".AddQueryString("start", start.ToString());
                url = url.AddQueryString("count", count.ToString());
                url = url.AddQueryString("filterByName", query);
                var result = GetPagedList<UserDetailContract>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<UserContract> GetUsersByRole(int roleId, int start, int count, string query)
        {
            try
            {
                var url = $"role/{roleId}/user".AddQueryString("start", start.ToString());
                url = url.AddQueryString("count", count.ToString());
                url = url.AddQueryString("filterByName", query);
                var result = GetPagedList<UserContract>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateUser(int userId, UpdateUserContract data)
        {
            try
            {
                Put<object>($"user/{userId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateUserContact(int userId, UpdateUserContactContract data)
        {
            try
            {
                Put<object>($"user/{userId}/contact", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void AddUserToRole(int userId, int roleId)
        {
            try
            {
                Post<object>($"role/{roleId}/user/{userId}", null);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void RemoveUserFromRole(int userId, int roleId)
        {
            try
            {
                Delete($"role/{roleId}/user/{userId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public bool ConfirmUserContact(ConfirmUserContactContract data)
        {
            try
            {
                return Post<bool>($"user/current/contact/confirmation", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void ResendConfirmCode(UserContactContract data)
        {
            try
            {
                Post<object>($"user/current/contact/confirmation/resend", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void SetTwoFactor(UpdateTwoFactorContract data)
        {
            try
            {
                Put<object>($"user/current/two-factor", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void SelectTwoFactorProvider(UpdateTwoFactorProviderContract data)
        {
            try
            {
                Put<object>($"user/current/two-factor/provider", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        public PagedResultList<RoleContract> GetRoleList(int start, int count, string query)
        {
            try
            {
                var url = "role".AddQueryString("start", start.ToString());
                url = url.AddQueryString("count", count.ToString());
                url = url.AddQueryString("filterByName", query);
                var result = GetPagedList<RoleContract>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<RoleContract> GetRoleAutocomplete(string query)
        {
            try
            {
                var result = Get<List<RoleContract>>("role/autocomplete".AddQueryString("query", query));
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public RoleDetailContract GetRoleDetail(int roleId)
        {
            try
            {
                var result = Get<RoleDetailContract>($"role/{roleId}/detail");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateRole(int roleId, RoleContract data)
        {
            try
            {
                Put<object>($"role/{roleId}", data);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateRole(RoleContract request)
        {
            try
            {
                var result = Post<int>("role", request);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteRole(int roleId)
        {
            try
            {
                Delete($"role/{roleId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<RoleContract> GetRolesByUser(int userId)
        {
            try
            {
                var result = Get<List<RoleContract>>($"user/{userId}/role");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<BookContract> GetAllBooksByType(BookTypeEnumContract bookType)
        {
            try
            {
                var result = Get<List<BookContract>>($"book/type/{bookType}/all");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<BookContract> GetBooksForRole(int roleId, BookTypeEnumContract bookType)
        {
            try
            {
                var result = Get<List<BookContract>>($"role/{roleId}/book?filterByBookType={bookType}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void AddBooksToRole(int roleId, IList<long> bookIds)
        {
            try
            {
                Post<object>($"role/{roleId}/permission/book", new AddBookToRoleRequestContract
                {
                    BookIdList = bookIds
                });
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void RemoveBooksFromRole(int roleId, IList<long> bookIds)
        {
            try
            {
                Delete($"role/{roleId}/permission/book", new AddBookToRoleRequestContract
                {
                    BookIdList = bookIds
                });
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public List<PermissionContract> GetPermissionsForRole(int roleId)
        {
            try
            {
                var result = Get<List<PermissionContract>>($"role/{roleId}/permission");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<PermissionContract> GetPermissions(int start, int count, string query)
        {
            try
            {
                var url = "permission".AddQueryString("start", start.ToString());
                url = url.AddQueryString("count", count.ToString());
                url = url.AddQueryString("filterByName", query);
                var result = GetPagedList<PermissionContract>(url);
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void AddSpecialPermissionsToRole(int roleId, IList<int> specialPermissionsIds)
        {
            try
            {
                Post<object>($"role/{roleId}/permission/special", new IntegerIdListContract
                {
                    IdList = specialPermissionsIds
                });
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void RemoveSpecialPermissionsFromRole(int roleId, IList<int> specialPermissionsIds)
        {
            try
            {
                Delete($"role/{roleId}/permission/special", new IntegerIdListContract
                {
                    IdList = specialPermissionsIds
                });
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void EnsureAuthServiceHasRequiredPermissions()
        {
            try
            {
                Put<object>($"permission/ensure", null);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        public List<BookTypeContract> GetBookTypeList()
        {
            try
            {
                var result = Get<List<BookTypeContract>>("book/type");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #region ExternalRepository

        public IList<ExternalRepositoryContract> GetAllExternalRepositories()
        {
            try
            {
                var result = Get<IList<ExternalRepositoryContract>>($"externalRepository/allExternalRepositories");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<ExternalRepositoryContract> GetExternalRepositoryList(int start, int count, bool fetchPageCount = false)
        {
            try
            {
                var result =
                    GetPagedList<ExternalRepositoryContract>(
                        $"externalRepository?start={start}&count={count}&fetchPageCount={fetchPageCount}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public ExternalRepositoryDetailContract GetExternalRepositoryDetail(int externalRepositoryId)
        {
            try
            {
                var result = Get<ExternalRepositoryDetailContract>($"externalRepository/{externalRepositoryId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public ExternalRepositoryStatisticsContract GetExternalRepositoryStatistics(int externalRepositoryId)
        {
            try
            {
                var result = Get<ExternalRepositoryStatisticsContract>($"externalRepository/{externalRepositoryId}/statistics");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateExternalRepository(ExternalRepositoryDetailContract externalRepository)
        {
            try
            {
                var externalRepositoryId = Post<int>("externalRepository", externalRepository);
                return externalRepositoryId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteExternalRepository(int externalRepositoryId)
        {
            try
            {
                Delete($"externalRepository/{externalRepositoryId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateExternalRepository(int externalRepositoryId, ExternalRepositoryDetailContract externalRepository)
        {
            try
            {
                Put<object>($"externalRepository/{externalRepositoryId}", externalRepository);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void CancelImportTask(int externalRepositoryId)
        {
            try
            {
                Delete($"externalRepository/{externalRepositoryId}/importStatus");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public IList<ExternalRepositoryTypeContract> GetAllExternalRepositoryTypes()
        {
            try
            {
                return Get<IList<ExternalRepositoryTypeContract>>($"externalRepository/allExternalRepositoryTypes");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }  
        
        public OaiPmhRepositoryInfoContract GetOaiPmhRepositoryInfo(string url)
        {
            try
            {               
                return Get<OaiPmhRepositoryInfoContract>($"externalRepository/oaiPmhRepositoryInfo?url={url.EncodeQueryString()}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region FilteringExpressionSet

        public PagedResultList<FilteringExpressionSetDetailContract> GetFilteringExpressionSetList(int start, int count, bool fetchPageCount = false)
        {
            try
            {
                var result =
                    GetPagedList<FilteringExpressionSetDetailContract>(
                        $"filteringExpressionSet?start={start}&count={count}&fetchPageCount={fetchPageCount}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }
        
        public IList<FilteringExpressionSetContract> GetAllFilteringExpressionSets()
        {
            try
            {
                var result = Get<IList<FilteringExpressionSetContract>>($"filteringExpressionSet/allFilteringExpressionSets");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public FilteringExpressionSetDetailContract GetFilteringExpressionSetDetail(int filteringExpressionSetId)
        {
            try
            {
                var result = Get<FilteringExpressionSetDetailContract>($"filteringExpressionSet/{filteringExpressionSetId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateFilteringExpressionSet(FilteringExpressionSetDetailContract filteringExpressionSet)
        {
            try
            {
                var filteringExpressionSetId = Post<int>("filteringExpressionSet", filteringExpressionSet);
                return filteringExpressionSetId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteFilteringExpressionSet(int filteringExpressionSetId)
        {
            try
            {
                Delete($"filteringExpressionSet/{filteringExpressionSetId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateFilteringExpressionSet(int filteringExpressionSetId, FilteringExpressionSetDetailContract filteringExpressionSet)
        {
            try
            {
                Put<object>($"filteringExpressionSet/{filteringExpressionSetId}", filteringExpressionSet);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public IList<BibliographicFormatContract> GetAllBibliographicFormats()
        {
            try
            {
                return Get<IList<BibliographicFormatContract>>($"filteringExpressionSet/allBibliographicFormats");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion

        #region Import

        public void StartImport(IList<int> externalRepositoryIds)
        {
            try
            {
                Post<object>($"repositoryImport", externalRepositoryIds);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        public IList<RepositoryImportProgressInfoContract> GetImportStatus()
        {
            try
            {
                return Get<IList<RepositoryImportProgressInfoContract>>($"repositoryImport/importStatus");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", GetCurrentMethod(), e);

                throw;
            }
        }

        #endregion
    }
}