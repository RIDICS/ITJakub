using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vokabular.Authentication.Client.SharedClient.Exceptions;

namespace Vokabular.Authentication.Client.SharedClient.Client
{
    public abstract class ServiceHttpClientBase<TContractException, TServiceException, TServiceApiException> : System.Net.Http.HttpClient
        where TContractException : class
        where TServiceException : ServiceException
        where TServiceApiException : ServiceApiException
    {
        protected readonly ILogger m_logger;
        
        protected ServiceHttpClientBase(
            string serviceAddress,
            ILogger logger
        )
        {
            m_logger = logger;
            BaseAddress = new Uri(serviceAddress);
        }

        protected ServiceHttpClientBase(
            Uri serviceAddress,
            ILogger logger
        )
        {
            m_logger = logger;
            BaseAddress = serviceAddress;
        }

        protected virtual NameValueCollection CreateQuery()
        {
            return HttpUtility.ParseQueryString(string.Empty);
        }

        protected virtual NameValueCollection CreateListedQuery(int start, int count, string search)
        {
            var parameters = CreateQuery();

            parameters.Add("start", start.ToString());
            parameters.Add("count", count.ToString());

            if (!string.IsNullOrEmpty(search))
            {
                parameters.Add("search", search);
            }

            return parameters;
        }

        public virtual Task<T> SendRequestAsync<T>(HttpMethod httpMethod, string url, object content = null)
        {
            return SendRequestAsync<T>(httpMethod, new Uri(url, UriKind.Relative), content);
        }

        public virtual async Task<T> SendRequestAsync<T>(HttpMethod httpMethod, Uri url, object content = null)
        {
            var result = await SendRequestAsync(httpMethod, url, content);
            return await GetDeserializedModelAsync<T>(result);
        }

        public virtual Task<HttpResponseMessage> SendRequestAsync(HttpMethod httpMethod, string url, object content = null)
        {
            return SendRequestAsync(httpMethod, new Uri(url, UriKind.Relative), content);
        }

        public virtual async Task<HttpResponseMessage> SendRequestAsync(HttpMethod httpMethod, Uri url, object content = null)
        {
            var request = new HttpRequestMessage(httpMethod, url)
            {
                Method = httpMethod
            };

            if (httpMethod == HttpMethod.Put || httpMethod == HttpMethod.Post)
            {
                var output = JsonConvert.SerializeObject(content);
                request.Content = new StringContent(output, Encoding.UTF8, "application/json");
            }

            await InsertCustomHeadersAsync(request);

            HttpResponseMessage responseMessage;
            try
            {
                responseMessage = await SendAsync(request);
            }
            catch (HttpRequestException exception)
            {
                var requestErrorMessage = "Error sending HTTP request";
                var newException = CreateServiceException(requestErrorMessage, exception);

                if (m_logger.IsEnabled(LogLevel.Error))
                    m_logger.LogError(exception, requestErrorMessage);

                throw newException;
            }

            if (responseMessage.IsSuccessStatusCode)
            {
                return responseMessage;
            }

            var errorMessage =
                $"Communication with {ServiceName} service ({httpMethod} {url}) failed with status {(int) responseMessage.StatusCode}";
            if (m_logger.IsEnabled(LogLevel.Error))
                m_logger.LogError(errorMessage);

            var stringContent = await responseMessage.Content.ReadAsStringAsync();

            var serviceException = CreateServiceException(errorMessage);

            serviceException.StatusCode = (int) responseMessage.StatusCode;
            serviceException.Content = stringContent;
            serviceException.ContentType = responseMessage.Content.Headers.ContentType?.MediaType;

            TryResolveException(stringContent, serviceException);

            throw serviceException;
        }

        protected virtual void TryResolveException(string content, TServiceException serviceException)
        {
            try
            {
                var contractException = GetDeserializedModel<TContractException>(content);

                if (!IsContractExceptionValid(contractException))
                {
                    var serviceResponse = GetDeserializedModel<ServiceResponse<TContractException>>(content);
                    contractException = serviceResponse?.Value;
                }

                if (IsContractExceptionValid(contractException))
                {
                    var serviceApiException = CreateServiceApiException(serviceException, contractException);

                    throw serviceApiException;
                }
            }
            catch (JsonException)
            {
            }
        }

        public virtual async Task<T> GetDeserializedModelAsync<T>(HttpResponseMessage responseMessage)
        {
            var stream = await responseMessage.Content.ReadAsStreamAsync();

            if (stream == null || stream.CanRead == false)
            {
                return default;
            }

            using (var sr = new StreamReader(stream))
            {
                return GetDeserializedJson<T>(sr);
            }
        }

        public virtual T GetDeserializedModel<T>(string response)
        {
            using (var textReader = new StringReader(response))
            {
                return GetDeserializedJson<T>(textReader);
            }
        }

        protected virtual T GetDeserializedJson<T>(TextReader textReader)
        {
            using (var jtr = new JsonTextReader(textReader))
            {
                var js = new JsonSerializer();

                return js.Deserialize<T>(jtr);
            }
        }

        protected virtual async Task<string> StreamToStringAsync(Stream stream)
        {
            string content = null;

            if (stream != null)
                using (var sr = new StreamReader(stream))
                    content = await sr.ReadToEndAsync();

            return content;
        }

        protected abstract string ServiceName { get; }

        protected abstract TServiceException CreateServiceException(string errorMessage, Exception innerException = null);

        protected abstract TServiceApiException CreateServiceApiException(TServiceException serviceException,
            TContractException contractException);

        protected abstract bool IsContractExceptionValid(TContractException ex);

        protected abstract Task InsertCustomHeadersAsync(HttpRequestMessage request);
    }
}