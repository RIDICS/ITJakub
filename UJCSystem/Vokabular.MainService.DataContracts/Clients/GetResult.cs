using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Vokabular.MainService.DataContracts.Headers;

namespace Vokabular.MainService.DataContracts.Clients
{
    public class GetResult<T>
    {
        public GetResult(T result, HttpResponseHeaders headers)
        {
            Result = result;
            ResponseHeaders = headers;
        }

        public T Result { get; }

        public HttpResponseHeaders ResponseHeaders { get; }

        public int GetTotalCountHeader()
        {
            IEnumerable<string> values;
            if (ResponseHeaders.TryGetValues(CustomHttpHeaders.TotalCount, out values))
            {
                return Convert.ToInt32(values.First());
            }

            return -1;
        }
    }
}
