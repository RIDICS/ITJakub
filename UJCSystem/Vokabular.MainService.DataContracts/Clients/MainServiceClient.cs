using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class MainServiceClient : IDisposable
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

        public IList<ProjectContract> GetProjectList()
        {
            var projectList = Get<List<ProjectContract>>("project");
            return projectList;
        }

        public ProjectContract GetProject(long projectId)
        {
            var project = Get<ProjectContract>(string.Format("project/{0}", projectId));
            return project;
        }

        public long CreateProject(ProjectContract project)
        {
            var projectId = Post<long>("project", project);
            return projectId;
        }

        public void DeleteProject(long projectId)
        {
            Delete(string.Format("project/{0}", projectId));
        }

        public void UploadResource(string sessionId, Stream data)
        {
            var uriPath = string.Format("session/{0}/resource", sessionId);
            var content = new StreamContent(data);
            var response = m_client.PostAsync(uriPath, content).Result;

            response.EnsureSuccessStatusCode();
        }

        public void Dispose()
        {
            m_client.Dispose();
        }
    }
}
