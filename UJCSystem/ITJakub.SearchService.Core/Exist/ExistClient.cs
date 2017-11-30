using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ITJakub.SearchService.Core.Exist.Attributes;
using Jewelery;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistClient
    {
        private readonly HttpClient m_httpClient;


        public ExistClient(ExistConnectionSettingsSkeleton connectionSettings)
        {
            var clientHandler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(connectionSettings.DbUser, connectionSettings.DbPassword)
            };
            m_httpClient = new HttpClient(clientHandler);
        }

        public Task<HttpContent> SendRequestAsync(CommunicationInfo commInfo, Uri uri, HttpContent content)
        {
            var httpRequestMessage = new HttpRequestMessage(new HttpMethod(commInfo.Method.GetStringValue()), uri);

            if (commInfo.Method.Equals(HttpMethodType.Post) || commInfo.Method.Equals(HttpMethodType.Put))
            {
                httpRequestMessage.Content = content;
            }


            return Task.Run(() => m_httpClient.SendAsync(httpRequestMessage))
                .ContinueWith(task =>
                {
                    //var message = Task.Run(() => task.Result.Content.ReadAsStringAsync()).Result; //content message for debug purposes
                    task.Result.EnsureSuccessStatusCode();
                    return task.Result.Content;
                });
        }

        public Task<string> SendRequestGetResponseAsStringAsync(CommunicationInfo commInfo, Uri uri, HttpContent content)
        {
            return SendRequestAsync(commInfo, uri, content).Result.ReadAsStringAsync();
        }

        public Task<Stream> SendRequestGetResponseAsStreamAsync(CommunicationInfo commInfo, Uri uri, HttpContent content)
        {
            return SendRequestAsync(commInfo, uri, content).Result.ReadAsStreamAsync();
        }

        public Task<byte[]> SendRequestGetResponseAsByteArrayAsync(CommunicationInfo commInfo, Uri uri, HttpContent content)
        {
            return SendRequestAsync(commInfo, uri, content).Result.ReadAsByteArrayAsync();
        }
    }
}