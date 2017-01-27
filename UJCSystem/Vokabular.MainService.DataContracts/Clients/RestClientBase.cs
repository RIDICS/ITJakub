using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Vokabular.MainService.DataContracts.Clients
{
    public abstract class RestClientBase : IDisposable
    {
        private readonly HttpClient m_client;

        public RestClientBase(Uri baseAddress)
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

        protected HttpClient HttpClient
        {
            get { return m_client; }
        }

        private T GetResponse<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var result = response.Content.ReadAsAsync<T>().Result;
            return result;
        }

        protected GetResult<T> GetFull<T>(string uriPath)
        {
            var response = m_client.GetAsync(uriPath).Result;
            var result = GetResponse<T>(response);

            return new GetResult<T>(result, response.Headers);
        }

        protected T Get<T>(string uriPath)
        {
            var response = m_client.GetAsync(uriPath).Result;
            return GetResponse<T>(response);
        }

        protected T Post<T>(string uriPath, object jsonData)
        {
            var response = m_client.PostAsJsonAsync(uriPath, jsonData).Result;
            return GetResponse<T>(response);
        }

        protected void Post(string uriPath, object jsonData)
        {
            var response = m_client.PostAsJsonAsync(uriPath, jsonData).Result;
            response.EnsureSuccessStatusCode();
        }

        protected void Put(string uriPath, object jsonData)
        {
            var response = m_client.PutAsJsonAsync(uriPath, jsonData).Result;
            response.EnsureSuccessStatusCode();
        }

        protected void Delete(string uriPath)
        {
            var response = m_client.DeleteAsync(uriPath).Result;
            response.EnsureSuccessStatusCode();
        }
    }
}
