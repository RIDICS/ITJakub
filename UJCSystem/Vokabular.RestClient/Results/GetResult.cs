using System.Net.Http.Headers;

namespace Vokabular.RestClient.Results
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
    }
}
