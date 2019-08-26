using System.Net;
using System.Net.Http.Headers;
using Vokabular.RestClient;

namespace Vokabular.CardFile.Core
{
    public class CardFilesRestClient : FullRestClient
    {
        public CardFilesRestClient(CardFilesCommunicationConfiguration configuration) : base(configuration)
        {
            var networkCredentials = new NetworkCredential(configuration.Username, configuration.Password);
            var credCache = new CredentialCache {{configuration.Url, "Digest", networkCredentials}};

            HttpClientHandler.Credentials = credCache;

            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            DeserializationType = DeserializationType.Xml;
        }
    }
}
