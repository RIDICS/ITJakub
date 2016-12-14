using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.MainService.DataContracts.ServiceContracts;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceClient : IDisposable, IProjectMainService, IResourceMainService, ISnapshotMainService
    {
        private readonly HttpClient m_client;

        public MainServiceClient(Uri baseAddress)
        {
            m_client = new HttpClient
            {
                BaseAddress = baseAddress
            };
            m_client.DefaultRequestHeaders.Accept.Clear();
            m_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void Dispose()
        {
            m_client.Dispose();
        }

        #region Generic methods

        private T GetResponse<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var result = response.Content.ReadAsAsync<T>().Result;
            return result;
        }

        private T Get<T>(string uriPath)
        {
            var response = m_client.GetAsync(uriPath).Result;
            return GetResponse<T>(response);
        }

        private T Post<T>(string uriPath, object data)
        {
            var response = m_client.PostAsJsonAsync(uriPath, data).Result;
            return GetResponse<T>(response);
        }

        private void Put(string uriPath, object data)
        {
            var response = m_client.PutAsJsonAsync(uriPath, data).Result;
            response.EnsureSuccessStatusCode();
        }

        private void Delete(string uriPath)
        {
            var response = m_client.DeleteAsync(uriPath).Result;
            response.EnsureSuccessStatusCode();
        }

        #endregion

        public List<ProjectContract> GetProjectList()
        {
            var projectList = Get<List<ProjectContract>>("project");
            return projectList;
        }

        public ProjectContract GetProject(long projectId)
        {
            var project = Get<ProjectContract>($"project/{projectId}");
            return project;
        }

        public long CreateProject(ProjectContract project)
        {
            var projectId = Post<long>("project", project);
            return projectId;
        }

        public void DeleteProject(long projectId)
        {
            Delete($"project/{projectId}");
        }

        public ProjectMetadataContract GetProjectMetadata(long projectId)
        {
            var metadata = Get<ProjectMetadataContract>($"project/{projectId}/metadata");
            return metadata;
        }

        public void UploadResource(string sessionId, Stream data)
        {
            var uriPath = $"session/{sessionId}/resource";
            var content = new StreamContent(data);
            var response = m_client.PostAsync(uriPath, content).Result;

            response.EnsureSuccessStatusCode();
        }

        public void DeleteResource(long resourceId)
        {
            Delete($"resource/{resourceId}");
        }

        public long DuplicateResource(long resourceId)
        {
            var newResourceId = Post<long>($"resource/{resourceId}/duplicate", null);
            return newResourceId;
        }

        public List<ResourceContract> GetResourceList(long projectId, ResourceTypeContract? resourceType)
        {
            var url = $"project/{projectId}/resource";
            if (resourceType != null)
            {
                url = $"{url}?resourceType={resourceType}";
            }

            var result = Get<List<ResourceContract>>(url);
            return result;
        }

        public List<ResourceVersionContract> GetResourceVersionHistory(long resourceId)
        {
            var result = Get<List<ResourceVersionContract>>($"resource/{resourceId}/version");
            return result;
        }

        public ResourceMetadataContract GetResourceMetadata(long resourceId)
        {
            var result = Get<ResourceMetadataContract>($"resource/{resourceId}/metadata");
            return result;
        }

        public long ProcessUploadedResources(long projectId, NewResourceContract resourceInfo)
        {
            var resourceId = Post<long>($"project/{projectId}/resource", null);
            return resourceId;
        }

        public long ProcessUploadedResourceVersion(long resourceId, NewResourceContract resourceInfo)
        {
            var resourceVersionId = Post<long>($"resource/{resourceId}/version", null);
            return resourceVersionId;
        }

        public void RenameResource(long resourceId, ResourceContract resource)
        {
            Put($"resource/{resourceId}", resource);
        }

        public void UploadResource(string sessionId)
        {
            throw new NotSupportedException("Method without Stream parameter is not supported");
        }

        public long CreateSnapshot(long projectId)
        {
            var snapshotId = Post<long>($"project/{projectId}/snapshot", null);
            return snapshotId;
        }

        public List<SnapshotContract> GetSnapshotList(long projectId)
        {
            var result = Get<List<SnapshotContract>>($"project/{projectId}/snapshot");
            return result;
        }
    }
}
