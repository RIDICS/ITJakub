using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceResourceClient
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private readonly MainServiceRestClient m_client;

        public MainServiceResourceClient(MainServiceRestClient client)
        {
            m_client = client;
        }

        public void DeleteResource(long resourceId)
        {
            try
            {
                m_client.Delete($"resource/{resourceId}");
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long DuplicateResource(long resourceId)
        {
            try
            {
                var newResourceId = m_client.Post<long>($"resource/{resourceId}/duplicate", null);
                return newResourceId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public IList<ResourceVersionContract> GetResourceVersionHistory(long resourceId)
        {
            try
            {
                var result = m_client.Get<IList<ResourceVersionContract>>($"resource/{resourceId}/version");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public ResourceMetadataContract GetResourceMetadata(long resourceId)
        {
            try
            {
                var result = m_client.Get<ResourceMetadataContract>($"resource/{resourceId}/metadata");
                return result;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public long ProcessUploadedResourceVersion(long resourceId, NewResourceContract resourceInfo)
        {
            try
            {
                var resourceVersionId = m_client.Post<long>($"resource/{resourceId}/version", resourceInfo);
                return resourceVersionId;
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }

        public void RenameResource(long resourceId, ResourceContract resource)
        {
            try
            {
                m_client.Put<object>($"resource/{resourceId}", resource);
            }
            catch (HttpRequestException e)
            {
                if (m_logger.IsErrorEnabled())
                    m_logger.LogError("{0} failed with {1}", m_client.GetCurrentMethod(), e);

                throw;
            }
        }
    }
}
