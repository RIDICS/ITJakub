using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Vokabular.OaiPmhImportManager.Model;
using Vokabular.ProjectImport.Model.Exceptions;
using Vokabular.Shared.Options;

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
        private readonly int m_retryCount;
        private readonly int m_delay;
        private readonly string m_url;
        private readonly HttpClient m_httpClient;
        private readonly XmlSerializer m_oaiPmhXmlSerializer;
        private readonly string m_granularityFormat;


        public OaiPmhCommunicationClient(OaiPmhClientOption option, string url)
        {
            m_retryCount = option.RetryCount;
            m_delay = option.Delay;
            m_url = url;
            m_httpClient = new HttpClient();
            m_oaiPmhXmlSerializer = new XmlSerializer(typeof(OAIPMHType));
            m_granularityFormat = DateGranularity;
        }

        public async Task<OaiPmhRepositoryInfo> GetRepositoryInfoAsync()
        {
            var identify = await  IdentifyAsync();
            var metadataFormats = await GetMetadataFormatsListAsync();
            var sets = await GetSetsListAsync(true);

            return new OaiPmhRepositoryInfo
            {
                AdminMails = identify.adminEmail,
                Description = identify.description?.ToString(),
                Url = identify.baseURL,
                EarliestDateTime = DateTime.Parse(identify.earliestDatestamp),
                Granularity = identify.granularity,
                Name = identify.repositoryName,
                MetadataFormatTypes = metadataFormats,
                SetTypes = sets
            };
        }

        public async Task<IdentifyType> IdentifyAsync()
        {
            return await GetVerbAsync<IdentifyType>(verbType.Identify);
        }

        public async Task<IList<metadataFormatType>> GetMetadataFormatsListAsync()
        {
            var list = await GetVerbAsync<ListMetadataFormatsType>(verbType.ListMetadataFormats);
            return list.metadataFormat.ToList();
        }

        public Task<ListSetsType> GetSetsListAsync(string resumptionToken)
        {
            return GetResumptionTokenAsync<ListSetsType>(verbType.ListIdentifiers, resumptionToken);
        }

        public async Task<List<setType>> GetSetsListAsync(bool fetchCompleteList = false)
        {
            var listSets = await GetVerbAsync<ListSetsType>(verbType.ListSets);
            var resumptionToken = listSets.resumptionToken;

            var list = new List<setType>(listSets.set);

            while (fetchCompleteList && resumptionToken != null)
            {
                listSets = await GetSetsListAsync(resumptionToken.Value);
                list.AddRange(listSets.set);
                resumptionToken = listSets.resumptionToken;
            }

            return list;
        }

        public async Task<ListIdentifiersType> GetListIdentifiersTypeAsync(string format, string set)
        {
            return await GetVerbAsync<ListIdentifiersType>(verbType.ListIdentifiers, format, set);
        }

        public async Task<ListIdentifiersType> GetListIdentifiersTypeAsync(string resumptionToken)
        {
            return await GetResumptionTokenAsync<ListIdentifiersType>(verbType.ListIdentifiers, resumptionToken);
        }

        public async Task<IList<headerType>> GetIdentifiersListAsync(string format, string set, bool fetchCompleteList = false)
        {
            var identifiers = await GetVerbAsync<ListIdentifiersType>(verbType.ListIdentifiers, format, set);
            var resumptionToken = identifiers.resumptionToken;
            var list = new List<headerType>(identifiers.header);

            while (fetchCompleteList && resumptionToken != null)
            {
                identifiers = await GetListIdentifiersTypeAsync(resumptionToken.Value);
                list.AddRange(identifiers.header);
                resumptionToken = identifiers.resumptionToken;
            }

            return list;
        }

        public async Task<IList<recordType>> GetRecordsListAsync(string format, string set, bool fetchCompleteList, DateTime? from = null,
            DateTime? until = null)
        {
            var records = await GetVerbAsync<ListRecordsType>(verbType.ListRecords, format, set, from, until);
            var resumptionToken = records.resumptionToken;
            var list = new List<recordType>(records.record);

            while (fetchCompleteList && resumptionToken != null)
            {
                records = await GetRecordsListAsync(resumptionToken.Value);
                list.AddRange(records.record);
                resumptionToken = records.resumptionToken;
            }

            return list;
        }

        public async Task<ListRecordsType> GetRecordsListAsync(string format, string set, DateTime? from = null,
            DateTime? until = null)
        {
            return await GetVerbAsync<ListRecordsType>(verbType.ListRecords, format, set, from, until);
        }


        public async Task<ListRecordsType> GetRecordsListAsync(string resumptionToken)
        {
            return await GetResumptionTokenAsync<ListRecordsType>(verbType.ListRecords, resumptionToken);
        }

        public async Task<recordType> GetRecordAsync(string format, string identifier)
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
            var currentRetry = 0;

            for (;;)
            {
                var uriBuilder = new UriBuilder(m_url) { Query = query };

                try
                {
                    
                    var streamResult = await m_httpClient.GetStreamAsync(uriBuilder.Uri);
                    var oaiPmhRecordResponse = (OAIPMHType) m_oaiPmhXmlSerializer.Deserialize(streamResult);

                    //Validate response
                    if (oaiPmhRecordResponse.Items.First().GetType() == typeof(OAIPMHerrorType))
                    {
                        var error = (OAIPMHerrorType) oaiPmhRecordResponse.Items.First();
                        throw new ImportFailedException(
                            $"Error while requesting: {uriBuilder.Uri}. {error.code} : {error.Value}");
                    }

                    return oaiPmhRecordResponse;
                }
                catch (HttpRequestException e)
                {
                    currentRetry++;

                    if (currentRetry > m_retryCount)
                    {
                        throw new ImportFailedException($"Error while requesting: {uriBuilder.Uri}. Error message: {e.Message}", e);
                    }
                }

                await Task.Delay(m_delay);
            }
        }

        private async Task<T> GetVerbAsync<T>(verbType verbType, string format = null, string set = null, DateTime? from = null,
            DateTime? until = null)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add(Verb, verbType.ToString());
            
            if(!string.IsNullOrEmpty(format))
                queryString.Add(MetadataPrefix, format);

            if (!string.IsNullOrEmpty(format))
                queryString.Add(Set, set);

            if (from.HasValue)
                queryString.Add(From, from.Value.ToString(m_granularityFormat));

            if (until.HasValue)
                queryString.Add(Until, until.Value.ToString(m_granularityFormat));

            return (T) (await GetResponseAsync(queryString.ToString())).Items.First();
        }

        private async Task<T> GetResumptionTokenAsync<T>(verbType verbType, string resumptionToken)
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