using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Extensions;

namespace Vokabular.RestClient
{
    public abstract class RestClientBase : IDisposable
    {
        private const int StreamBufferSize = 64 * 1024;
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

        protected abstract void FillRequestMessage(HttpRequestMessage requestMessage);

        protected abstract void ProcessResponse(HttpResponseMessage response);

        public void Dispose()
        {
            m_client.Dispose();
        }

        protected HttpClient HttpClient
        {
            get { return m_client; }
        }

        private HttpRequestMessage CreateRequestMessage(HttpMethod method, string requestUri, IEnumerable<Tuple<string, string>> headers = null)
        {
            var requestMessage = new HttpRequestMessage(method, requestUri);

            if (headers != null)
            {
                foreach (var headerValuepair in headers)
                    requestMessage.Headers.Add(headerValuepair.Item1, headerValuepair.Item2);
            }

            FillRequestMessage(requestMessage);

            return requestMessage;
        }

        private void ProcessResponseInternal(HttpResponseMessage response)
        {
            EnsureSuccessStatusCode(response);
            
            ProcessResponse(response);
        }

        private T GetResponseBody<T>(HttpResponseMessage response)
        {
            try
            {
                var result = response.Content.ReadAsAsync<T>().GetAwaiter().GetResult();
                return result;
            }
            catch (JsonSerializationException exception)
            {
                throw new HttpErrorCodeException("Invalid response JSON format", exception, HttpStatusCode.BadGateway);
            }
            catch (JsonReaderException exception)
            {
                throw new HttpErrorCodeException("Invalid response JSON format", exception, HttpStatusCode.BadGateway);
            }
        }

        protected Task<GetResult<T>> GetFullAsync<T>(string uriPath)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var request = CreateRequestMessage(HttpMethod.Get, uriPath);
                    var response = await m_client.SendAsync(request);

                    ProcessResponseInternal(response);
                    var result = GetResponseBody<T>(response);

                    return new GetResult<T>(result, response.Headers);
                }
                catch (TaskCanceledException e)
                {
                    throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                }
            });
        }

        protected Task<T> GetAsync<T>(string uriPath)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var request = CreateRequestMessage(HttpMethod.Get, uriPath);
                    var response = await m_client.SendAsync(request);

                    ProcessResponseInternal(response);
                    return GetResponseBody<T>(response);
                }
                catch (TaskCanceledException e)
                {
                    throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                }
            });
        }

        protected Task<string> GetStringAsync(string uriPath)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var request = CreateRequestMessage(HttpMethod.Get, uriPath);
                    var response = await m_client.SendAsync(request);

                    ProcessResponseInternal(response);
                    return await response.Content.ReadAsStringAsync();
                }
                catch (TaskCanceledException e)
                {
                    throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                }
            });
        }

        protected Task<Stream> GetStreamAsync(string uriPath)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var request = CreateRequestMessage(HttpMethod.Get, uriPath);
                    var response = await m_client.SendAsync(request);

                    ProcessResponseInternal(response);
                    return await response.Content.ReadAsStreamAsync();
                }
                catch (TaskCanceledException e)
                {
                    throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                }
            });
        }

        protected Task HeadAsync(string uriPath)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var request = CreateRequestMessage(HttpMethod.Head, uriPath);
                    var response = await m_client.SendAsync(request);

                    ProcessResponseInternal(response);
                }
                catch (TaskCanceledException e)
                {
                    throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                }
            });
        }

        protected Task<T> PostAsync<T>(string uriPath, object data)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var request = CreateRequestMessage(HttpMethod.Post, uriPath);
                    var response = await m_client.SendAsJsonAsync(request, data);

                    ProcessResponseInternal(response);

                    return GetResponseBody<T>(response);
                }
                catch (TaskCanceledException e)
                {
                    throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                }
            });
        }

        protected Task<T> PostStreamAsFormAsync<T>(string uriPath, Stream data, IEnumerable<Tuple<string, string>> headers = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var content = new MultipartFormDataContent();
                    content.Add(new StreamContent(data, StreamBufferSize), "file");

                    var request = CreateRequestMessage(HttpMethod.Post, uriPath, headers);
                    request.Content = content;

                    var response = await m_client.SendAsync(request);

                    ProcessResponseInternal(response);
                    return GetResponseBody<T>(response);
                }
                catch (TaskCanceledException e)
                {
                    throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                }
            });
        }

        protected Task<T> PostStreamAsync<T>(string uriPath, Stream data, IEnumerable<Tuple<string, string>> headers = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var content = new StreamContent(data, StreamBufferSize);

                    var request = CreateRequestMessage(HttpMethod.Post, uriPath, headers);
                    request.Content = content;

                    var response = await m_client.SendAsync(request);

                    ProcessResponseInternal(response);
                    return GetResponseBody<T>(response);
                }
                catch (TaskCanceledException e)
                {
                    throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                }
            });
        }

        protected Task<T> PutAsync<T>(string uriPath, object data)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var request = CreateRequestMessage(HttpMethod.Put, uriPath);
                    var response = await m_client.SendAsJsonAsync(request, data);

                    ProcessResponseInternal(response);
                    return GetResponseBody<T>(response);
                }
                catch (TaskCanceledException e)
                {
                    throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                }
            });
        }

        protected Task DeleteAsync(string uriPath)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var request = CreateRequestMessage(HttpMethod.Delete, uriPath);
                    var response = await m_client.SendAsync(request);

                    ProcessResponseInternal(response);
                }
                catch (TaskCanceledException e)
                {
                    throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                }
            });
        }

        private void EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpErrorCodeException(GetExceptionMessageFromResponse(response), response.StatusCode);
            }
        }

        private string GetExceptionMessageFromResponse(HttpResponseMessage response)
        {
            return string.Format("{0} ({1})", response.ReasonPhrase, (int)response.StatusCode);
        }

        protected string GetCurrentMethod([CallerMemberName] string methodName = null)
        {
            return methodName;
        }

        protected HttpStatusCode GetErrorCode(HttpRequestException exception)
        {
            var exceptionWithCode = exception as HttpErrorCodeException;
            return exceptionWithCode != null ? exceptionWithCode.StatusCode : 0;
        }
    }
}
