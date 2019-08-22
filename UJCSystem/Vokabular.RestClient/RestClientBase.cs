using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vokabular.RestClient.Contracts;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Extensions;
using Vokabular.RestClient.Headers;
using Vokabular.RestClient.Results;

namespace Vokabular.RestClient
{
    public abstract class RestClientBase : IDisposable
    {
        private readonly TimeSpan m_defaultTimeout;
        private const int StreamBufferSize = 64 * 1024;

        protected RestClientBase(ServiceCommunicationConfiguration communicationConfiguration)
        {
            if (communicationConfiguration.CreateCustomHandler)
            {
                HttpClientHandler = new HttpClientHandler();
                HttpClient = new HttpClient(HttpClientHandler);
            }
            else
            {
                HttpClient = new HttpClient();
            }

            m_defaultTimeout = HttpClient.Timeout;

            HttpClient.BaseAddress = communicationConfiguration.Url;
            HttpClient.DefaultRequestHeaders.ExpectContinue = false;
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpClient.Timeout = Timeout.InfiniteTimeSpan;
            DeserializationType = DeserializationType.Json;
        }

        protected abstract void FillRequestMessage(HttpRequestMessage requestMessage);

        protected abstract void ProcessResponse(HttpResponseMessage response);

        protected abstract void TryParseResponseError(HttpStatusCode responseStatusCode, string responseContent);

        protected HttpClient HttpClient { get; }

        protected HttpClientHandler HttpClientHandler { get; }

        public DeserializationType DeserializationType { get; set; }

        private CancellationTokenSource CreateCancellationTokenSource(TimeSpan? timeout = null)
        {
            return new CancellationTokenSource(timeout ?? m_defaultTimeout);
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
                T result;
                switch (DeserializationType)
                {
                    case DeserializationType.Json:
                        result = response.Content.ReadAsAsync<T>().GetAwaiter().GetResult();
                        break;
                    case DeserializationType.Xml:
                        result = response.Content.ReadXmlAsAsync<T>().GetAwaiter().GetResult();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

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
            catch (InvalidOperationException exception)
            {
                throw new HttpErrorCodeException("Invalid response XML format", exception, HttpStatusCode.BadGateway);
            }
        }

        protected Task<GetResult<T>> GetFullAsync<T>(string uriPath)
        {
            return Task.Run(async () =>
            {
                using (var cts = CreateCancellationTokenSource())
                {
                    try
                    {
                        var request = CreateRequestMessage(HttpMethod.Get, uriPath);
                        var response = await HttpClient.SendAsync(request, cts.Token);

                        ProcessResponseInternal(response);
                        var result = GetResponseBody<T>(response);

                        return new GetResult<T>(result, response.Headers);
                    }
                    catch (TaskCanceledException e)
                    {
                        throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                    }
                }

            });
        }

        protected Task<PagedResultList<T>> GetPagedListAsync<T>(string uriPath)
        {
            return Task.Run(async () =>
            {
                using (var cts = CreateCancellationTokenSource())
                {
                    try
                    {
                        var request = CreateRequestMessage(HttpMethod.Get, uriPath);
                        var response = await HttpClient.SendAsync(request, cts.Token);

                        ProcessResponseInternal(response);
                        var result = GetResponseBody<List<T>>(response);

                        return new PagedResultList<T>
                        {
                            TotalCount = response.Headers.GetTotalCountHeader(),
                            List = result,
                        };
                    }
                    catch (TaskCanceledException e)
                    {
                        throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                    }
                }
            });
        }

        protected Task<T> GetAsync<T>(string uriPath)
        {
            return Task.Run(async () =>
            {
                using (var cts = CreateCancellationTokenSource())
                {
                    try
                    {
                        var request = CreateRequestMessage(HttpMethod.Get, uriPath);
                        var response = await HttpClient.SendAsync(request, cts.Token);

                        ProcessResponseInternal(response);
                        return GetResponseBody<T>(response);
                    }
                    catch (TaskCanceledException e)
                    {
                        throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                    }
                }
            });
        }

        protected Task<string> GetStringAsync(string uriPath)
        {
            return Task.Run(async () =>
            {
                using (var cts = CreateCancellationTokenSource())
                {
                    try
                    {
                        var request = CreateRequestMessage(HttpMethod.Get, uriPath);
                        var response = await HttpClient.SendAsync(request, cts.Token);

                        ProcessResponseInternal(response);
                        return await response.Content.ReadAsStringAsync();
                    }
                    catch (TaskCanceledException e)
                    {
                        throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                    }
                }
            });
        }

        protected Task<FileResultData> GetStreamAsync(string uriPath)
        {
            return Task.Run(async () =>
            {
                using (var cts = CreateCancellationTokenSource())
                {
                    try
                    {
                        var request = CreateRequestMessage(HttpMethod.Get, uriPath);
                        var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);

                        ProcessResponseInternal(response);
                        var contentHeaders = response.Content.Headers;
                        var contentType = contentHeaders.ContentType;
                        var fileName = contentHeaders.ContentDisposition?.FileName;
                        var fileSize = contentHeaders.ContentLength;
                        var resultStream = await response.Content.ReadAsStreamAsync();

                        return new FileResultData
                        {
                            FileName = fileName,
                            FileSize = fileSize,
                            MimeType = contentType.MediaType,
                            Stream = resultStream,
                        };
                    }
                    catch (TaskCanceledException e)
                    {
                        throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                    }
                }
            });
        }

        protected Task HeadAsync(string uriPath)
        {
            return Task.Run(async () =>
            {
                using (var cts = CreateCancellationTokenSource())
                {
                    try
                    {
                        var request = CreateRequestMessage(HttpMethod.Head, uriPath);
                        var response = await HttpClient.SendAsync(request, cts.Token);

                        ProcessResponseInternal(response);
                    }
                    catch (TaskCanceledException e)
                    {
                        throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                    }
                }
            });
        }

        protected Task<T> PostAsync<T>(string uriPath, object data, TimeSpan? timeout = null)
        {
            return Task.Run(async () =>
            {
                using (var cts = CreateCancellationTokenSource(timeout))
                {
                    try
                    {
                        var request = CreateRequestMessage(HttpMethod.Post, uriPath);
                        var response = await HttpClient.SendAsJsonAsync(request, data, cts.Token);

                        ProcessResponseInternal(response);

                        return GetResponseBody<T>(response);
                    }
                    catch (TaskCanceledException e)
                    {
                        throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                    }
                }
            });
        }

        protected Task<T> PostStreamAsFormAsync<T>(string uriPath, Stream data, string fileName, IEnumerable<Tuple<string, string>> headers = null, TimeSpan? timeout = null)
        {
            return Task.Run(async () =>
            {
                using (var cts = CreateCancellationTokenSource(timeout))
                {
                    try
                    {
                        var fileStreamContent = new StreamContent(data, StreamBufferSize);
                        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(ContentTypes.ApplicationOctetStream);

                        var content = new MultipartFormDataContent();
                        content.Add(fileStreamContent, "File", fileName);

                        var request = CreateRequestMessage(HttpMethod.Post, uriPath, headers);
                        request.Content = content;
                        request.Headers.TransferEncodingChunked = true;

                        var response = await HttpClient.SendAsync(request, cts.Token);

                        ProcessResponseInternal(response);
                        return GetResponseBody<T>(response);
                    }
                    catch (TaskCanceledException e)
                    {
                        throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                    }
                }
            });
        }

        protected Task<T> PostStreamAsync<T>(string uriPath, Stream data, IEnumerable<Tuple<string, string>> headers = null, TimeSpan? timeout = null)
        {
            return Task.Run(async () =>
            {
                using (var cts = CreateCancellationTokenSource(timeout))
                {
                    try
                    {
                        var content = new StreamContent(data, StreamBufferSize);
                        content.Headers.ContentType = new MediaTypeHeaderValue(ContentTypes.ApplicationOctetStream);

                        var request = CreateRequestMessage(HttpMethod.Post, uriPath, headers);
                        request.Content = content;
                        request.Headers.TransferEncodingChunked = true;

                        var response = await HttpClient.SendAsync(request, cts.Token);

                        ProcessResponseInternal(response);
                        return GetResponseBody<T>(response);
                    }
                    catch (TaskCanceledException e)
                    {
                        throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                    }
                }
            });
        }

        protected Task<string> PostReturnStringAsync(string uriPath, object data, TimeSpan? timeout = null)
        {
            return Task.Run(async () =>
            {
                using (var cts = CreateCancellationTokenSource(timeout))
                {
                    try
                    {
                        var request = CreateRequestMessage(HttpMethod.Post, uriPath);
                        var response = await HttpClient.SendAsJsonAsync(request, data, cts.Token);

                        ProcessResponseInternal(response);
                        return await response.Content.ReadAsStringAsync();
                    }
                    catch (TaskCanceledException e)
                    {
                        throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                    }
                }
            });
        }

        protected Task<T> PutAsync<T>(string uriPath, object data, TimeSpan? timeout = null)
        {
            return Task.Run(async () =>
            {
                using (var cts = CreateCancellationTokenSource(timeout))
                {
                    try
                    {
                        var request = CreateRequestMessage(HttpMethod.Put, uriPath);
                        var response = await HttpClient.SendAsJsonAsync(request, data, cts.Token);

                        ProcessResponseInternal(response);
                        return GetResponseBody<T>(response);
                    }
                    catch (TaskCanceledException e)
                    {
                        throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                    }
                }
            });
        }

        protected Task DeleteAsync(string uriPath, object data = null)
        {
            return Task.Run(async () =>
            {
                using (var cts = CreateCancellationTokenSource())
                {
                    try
                    {
                        var request = CreateRequestMessage(HttpMethod.Delete, uriPath);
                        var response = data == null
                            ? await HttpClient.SendAsync(request, cts.Token)
                            : await HttpClient.SendAsJsonAsync(request, data, cts.Token);

                        ProcessResponseInternal(response);
                    }
                    catch (TaskCanceledException e)
                    {
                        throw new HttpErrorCodeException("Request timeout", e, HttpStatusCode.GatewayTimeout);
                    }
                }
            });
        }

        protected void EnsureSecuredClient()
        {
            if (HttpClient.BaseAddress.Scheme != "https")
            {
                throw new InvalidOperationException($"The client is not configured to use secured channel (HTTPS), current scheme is {HttpClient.BaseAddress.Scheme}");
            }
        }

        private void EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var responseStatusCode = response.StatusCode;
            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            TryParseResponseError(responseStatusCode, responseContent);

            if (responseStatusCode == HttpStatusCode.BadRequest &&
                TryDeserialize<ValidationResultContract>(responseContent, out var validationResult))
            {
                throw new HttpErrorCodeException(validationResult.Message, responseStatusCode, validationResult.Errors);
            }

            var exceptionMessage = responseContent.Trim('\"');
            throw new HttpErrorCodeException(exceptionMessage, responseStatusCode);
        }

        protected bool TryDeserialize<T>(string responseContent, out T validationResult)
        {
            validationResult = default(T);
            if (!(responseContent.StartsWith("{") && responseContent.EndsWith("}")))
            {
                return false;
            }

            try
            {
                var result = responseContent.Deserialize<T>();
                validationResult = result;
                return true;
            }
            catch (JsonSerializationException)
            {
                return false;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        public string GetCurrentMethod([CallerMemberName] string methodName = null)
        {
            return methodName;
        }

        protected HttpStatusCode GetErrorCode(HttpRequestException exception)
        {
            var exceptionWithCode = exception as HttpErrorCodeException;
            return exceptionWithCode != null ? exceptionWithCode.StatusCode : 0;
        }

        public void Dispose()
        {
            HttpClient?.Dispose();
            HttpClientHandler?.Dispose();
        }
    }
}