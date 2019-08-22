using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Vokabular.RestClient.Results;

namespace Vokabular.RestClient
{
    public class FullRestClient : RestClientBase
    {
        protected FullRestClient(ServiceCommunicationConfiguration communicationConfiguration) : base(communicationConfiguration)
        {
        }

        public GetResult<T> GetFull<T>(string uriPath)
        {
            return GetFullAsync<T>(uriPath).GetAwaiter().GetResult();
        }

        public PagedResultList<T> GetPagedList<T>(string uriPath)
        {
            return GetPagedListAsync<T>(uriPath).GetAwaiter().GetResult();
        }

        public T Get<T>(string uriPath)
        {
            return GetAsync<T>(uriPath).GetAwaiter().GetResult();
        }

        public string GetString(string uriPath)
        {
            return GetStringAsync(uriPath).GetAwaiter().GetResult();
        }

        public FileResultData GetStream(string uriPath)
        {
            return GetStreamAsync(uriPath).GetAwaiter().GetResult();
        }

        public void Head(string uriPath)
        {
            HeadAsync(uriPath).GetAwaiter().GetResult();
        }

        public T Post<T>(string uriPath, object data, TimeSpan? timeout = null)
        {
            return PostAsync<T>(uriPath, data, timeout).GetAwaiter().GetResult();
        }

        public T PostStreamAsForm<T>(string uriPath, Stream data, string fileName, IEnumerable<Tuple<string, string>> headers = null, TimeSpan? timeout = null)
        {
            return PostStreamAsFormAsync<T>(uriPath, data, fileName, headers, timeout).GetAwaiter().GetResult();
        }

        public T PostStream<T>(string uriPath, Stream data, IEnumerable<Tuple<string, string>> headers = null, TimeSpan? timeout = null)
        {
            return PostStreamAsync<T>(uriPath, data, headers, timeout).GetAwaiter().GetResult();
        }

        public string PostReturnString(string uriPath, object data, TimeSpan? timeout = null)
        {
            return PostReturnStringAsync(uriPath, data, timeout).GetAwaiter().GetResult();
        }

        public T Put<T>(string uriPath, object data, TimeSpan? timeout = null)
        {
            return PutAsync<T>(uriPath, data, timeout).GetAwaiter().GetResult();
        }

        public void Delete(string uriPath, object data = null)
        {
            DeleteAsync(uriPath, data).GetAwaiter().GetResult();
        }

        protected override void FillRequestMessage(HttpRequestMessage requestMessage)
        {
            
        }

        protected override void ProcessResponse(HttpResponseMessage response)
        {

        }

        protected override void TryParseResponseError(HttpStatusCode statusCode, string responseText)
        {

        }
    }
}