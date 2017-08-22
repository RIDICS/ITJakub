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
            return GetFullAsync<T>(uriPath).Result;
        }

        protected T Get<T>(string uriPath)
        {
            return GetAsync<T>(uriPath).Result;
        }

        protected Stream GetStream(string uriPath)
        {
            return GetStreamAsync(uriPath).Result;
        }

        protected void Head(string uriPath)
        {
            HeadAsync(uriPath).GetAwaiter().GetResult();
        }

        protected T Post<T>(string uriPath, object data)
        {
            return PostAsync<T>(uriPath, data).Result;
        }

        protected T PostStreamAsForm<T>(string uriPath, Stream data, IEnumerable<Tuple<string, string>> headers = null)
        {
            return PostStreamAsFormAsync<T>(uriPath, data, headers).Result;
        }

        protected T PostStream<T>(string uriPath, Stream data, IEnumerable<Tuple<string, string>> headers = null)
        {
            return PostStreamAsync<T>(uriPath, data, headers).Result;
        }

        protected T Put<T>(string uriPath, object data)
        {
            return PutAsync<T>(uriPath, data).Result;
        }

        protected void Delete(string uriPath)
        {
            DeleteAsync(uriPath).GetAwaiter().GetResult();
        }
    }
}