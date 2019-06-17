using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Vokabular.OaiPmhImportManager.Model;
using Vokabular.ProjectImport.Model.Exceptions;

namespace Vokabular.OaiPmhImportManager
{
    public class OaiPmhCommunicationClient : IDisposable
    {
        private const string Verb = "verb";
        private const string Set = "set";
        private const string MetadataPrefix = "metadataPrefix";
        private const string ResumptionToken = "resumptionToken";
        private const string From = "from";
        private const string Until = "until";
        private const string Identifier = "identifier";
        private const string DateGranularity = "yyyy-MM-dd";
        private readonly string m_url;
        private readonly bool m_disabledSslValidation;
        private readonly HttpClient m_httpClient;
        private readonly XmlSerializer m_oaiPmhXmlSerializer;
        private readonly string m_granularityFormat;


        public OaiPmhCommunicationClient(string url, bool disabledSslValidation)
        {
            m_url = url;
            m_disabledSslValidation = disabledSslValidation;
            m_httpClient = new HttpClient();
            m_oaiPmhXmlSerializer = new XmlSerializer(typeof(OAIPMHType));
            m_granularityFormat = DateGranularity;
        }

        protected async Task<recordType> GetRecordAsync(string format, string identifier)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add(Verb, verbType.GetRecord.ToString());

            if (!string.IsNullOrEmpty(format))
                queryString.Add(MetadataPrefix, format);

            if (!string.IsNullOrEmpty(identifier))
                queryString.Add(Identifier, identifier);


            var responseOai = await GetResponseAsync(queryString.ToString());
            return ((GetRecordType) responseOai.Items.First()).record;
        }

        private async Task<OAIPMHType> GetResponseAsync(string query)
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                if (m_disabledSslValidation)
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                }
                
                using (var client = new HttpClient(httpClientHandler))
                {
                    var uriBuilder = new UriBuilder(m_url) { Query = query };

                    var streamResult = await client.GetStreamAsync(uriBuilder.Uri);
                    var oaiPmhRecordResponse = (OAIPMHType)m_oaiPmhXmlSerializer.Deserialize(streamResult);

                    //Validate response
                    if (oaiPmhRecordResponse.Items.First().GetType() == typeof(OAIPMHerrorType))
                    {
                        var error = (OAIPMHerrorType)oaiPmhRecordResponse.Items.First();
                        throw new ImportFailedException(
                            $"Error while requesting: {uriBuilder.Uri}. {error.code} : {error.Value}");
                    }

                    return oaiPmhRecordResponse;
                }
            }
        }

        protected async Task<T> GetVerbAsync<T>(verbType verbType, string format = null, string set = null, DateTime? from = null,
            DateTime? until = null)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add(Verb, verbType.ToString());

            if (!string.IsNullOrEmpty(format))
                queryString.Add(MetadataPrefix, format);

            if (!string.IsNullOrEmpty(format))
                queryString.Add(Set, set);

            if (from.HasValue)
                queryString.Add(From, from.Value.ToString(m_granularityFormat));

            if (until.HasValue)
                queryString.Add(Until, until.Value.ToString(m_granularityFormat));

            return (T) (await GetResponseAsync(queryString.ToString())).Items.First();
        }

        protected async Task<T> GetResumptionTokenAsync<T>(verbType verbType, string resumptionToken)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add(Verb, verbType.ToString());

            if (!string.IsNullOrEmpty(resumptionToken))
                queryString.Add(ResumptionToken, resumptionToken);

            return (T) (await GetResponseAsync(queryString.ToString())).Items.First();
        }

        public void Dispose()
        {
            m_httpClient?.Dispose();
        }
    }
}