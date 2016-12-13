using System;
using System.Collections.Generic;
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

        public IList<ProjectContract> GetProjectList()
        {
            var response = m_client.GetAsync("project").Result;
            response.EnsureSuccessStatusCode();

            var projectList = response.Content.ReadAsAsync<List<ProjectContract>>().Result;
            return projectList;
        }

        public ProjectContract GetProject(long projectId)
        {
            var response = m_client.GetAsync(string.Format("project/{0}", projectId)).Result;
            response.EnsureSuccessStatusCode();

            var project = response.Content.ReadAsAsync<ProjectContract>().Result;
            return project;
        }

        public void Dispose()
        {
            m_client.Dispose();
        }
    }
}
