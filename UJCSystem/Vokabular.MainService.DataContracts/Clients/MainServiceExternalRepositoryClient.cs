using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.OaiPmh;
using Vokabular.RestClient.Extensions;
using Vokabular.RestClient.Results;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceExternalRepositoryClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private readonly MainServiceRestClient m_client;

        public MainServiceExternalRepositoryClient(MainServiceRestClient client)
        {
            m_client = client;
        }

        public IList<ExternalRepositoryContract> GetAllExternalRepositories()
        {
            try
            {
                var result = m_client.Get<IList<ExternalRepositoryContract>>($"bibliography/repository/all");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public PagedResultList<ExternalRepositoryContract> GetExternalRepositoryList(int start, int count, bool fetchPageCount = false)
        {
            try
            {
                var result =
                    m_client.GetPagedList<ExternalRepositoryContract>(
                        $"bibliography/repository?start={start}&count={count}&fetchPageCount={fetchPageCount}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public ExternalRepositoryDetailContract GetExternalRepositoryDetail(int externalRepositoryId)
        {
            try
            {
                var result = m_client.Get<ExternalRepositoryDetailContract>($"bibliography/repository/{externalRepositoryId}");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public ExternalRepositoryStatisticsContract GetExternalRepositoryStatistics(int externalRepositoryId)
        {
            try
            {
                var result = m_client.Get<ExternalRepositoryStatisticsContract>($"bibliography/repository/{externalRepositoryId}/statistics");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public int CreateExternalRepository(ExternalRepositoryDetailContract externalRepository)
        {
            try
            {
                var externalRepositoryId = m_client.Post<int>("bibliography/repository", externalRepository);
                return externalRepositoryId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void DeleteExternalRepository(int externalRepositoryId)
        {
            try
            {
                m_client.Delete($"bibliography/repository/{externalRepositoryId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void UpdateExternalRepository(int externalRepositoryId, ExternalRepositoryDetailContract externalRepository)
        {
            try
            {
                m_client.Put<object>($"bibliography/repository/{externalRepositoryId}", externalRepository);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void CancelImportTask(int externalRepositoryId)
        {
            try
            {
                m_client.Delete($"bibliography/repository/{externalRepositoryId}/import-status");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public IList<ExternalRepositoryTypeContract> GetAllExternalRepositoryTypes()
        {
            try
            {
                return m_client.Get<IList<ExternalRepositoryTypeContract>>($"bibliography/repository/type");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public OaiPmhRepositoryInfoContract GetOaiPmhRepositoryInfo(string url)
        {
            try
            {
                return m_client.Get<OaiPmhRepositoryInfoContract>($"bibliography/repository/external-info/oai-pmh?url={url.EncodeQueryString()}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }


        #region Import

        public void StartImport(IList<int> externalRepositoryIds)
        {
            try
            {
                m_client.Post<object>($"bibliography/import", externalRepositoryIds);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public IList<RepositoryImportProgressInfoContract> GetImportStatus()
        {
            try
            {
                return m_client.Get<IList<RepositoryImportProgressInfoContract>>($"bibliography/import/status");
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
