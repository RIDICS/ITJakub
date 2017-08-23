using System;
using System.Collections.Generic;
using System.IO;

namespace Vokabular.MainService.DataContracts.Clients
{
    public abstract class FullRestClientBase : RestClientBase
    {
        protected FullRestClientBase(Uri baseAddress) : base(baseAddress)
        {
        }

        protected GetResult<T> GetFull<T>(string uriPath)
        {
            return GetFullAsync<T>(uriPath).GetAwaiter().GetResult();
        }

        protected T Get<T>(string uriPath)
        {
            return GetAsync<T>(uriPath).GetAwaiter().GetResult();
        }

        protected Stream GetStream(string uriPath)
        {
            return GetStreamAsync(uriPath).GetAwaiter().GetResult();
        }

        protected void Head(string uriPath)
        {
            HeadAsync(uriPath).GetAwaiter().GetResult();
        }

        protected T Post<T>(string uriPath, object data)
        {
            return PostAsync<T>(uriPath, data).GetAwaiter().GetResult();
        }

        protected T PostStreamAsForm<T>(string uriPath, Stream data, IEnumerable<Tuple<string, string>> headers = null)
        {
            return PostStreamAsFormAsync<T>(uriPath, data, headers).GetAwaiter().GetResult();
        }

        protected T PostStream<T>(string uriPath, Stream data, IEnumerable<Tuple<string, string>> headers = null)
        {
            return PostStreamAsync<T>(uriPath, data, headers).GetAwaiter().GetResult();
        }

        protected T Put<T>(string uriPath, object data)
        {
            return PutAsync<T>(uriPath, data).GetAwaiter().GetResult();
        }

        protected void Delete(string uriPath)
        {
            DeleteAsync(uriPath).GetAwaiter().GetResult();
        }
    }
}