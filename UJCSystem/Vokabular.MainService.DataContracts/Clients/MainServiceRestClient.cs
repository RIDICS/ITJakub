using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient;
using Vokabular.RestClient.Extensions;
using Vokabular.RestClient.Results;
using Vokabular.Shared;
using Vokabular.Shared.DataContracts.Types;
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
    }
}