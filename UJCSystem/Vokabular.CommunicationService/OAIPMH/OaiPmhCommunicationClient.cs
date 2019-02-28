using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Vokabular.Shared.Options;

namespace Vokabular.CommunicationService.OAIPMH
{
    public class OaiPmhCommunicationClient : IDisposable
    {
        private string Url { get; }
        private const string Verb = "?verb=";
        private const string Set = "&set=";
        private const string MetadataPrefix = "&metadataPrefix=";
        private const string ResumptionToken = "&resumptionToken=";
        private readonly HttpClient m_httpClient;
        private readonly XmlSerializer m_oaiPmhXmlSerializer;
        private readonly int m_retryCount;
        private readonly int m_delay;

        public OaiPmhCommunicationClient(OaiPmhClientOption option, string url)
        {
            m_retryCount = option.RetryCount;
            m_delay = option.Delay;
            Url = url;
            m_httpClient = new HttpClient();
            m_oaiPmhXmlSerializer = new XmlSerializer(typeof(OAIPMHType));
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

        public async Task<IList<headerType>> GetIdentifiersList(string format, string set, bool fetchCompleteList = false)
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

        public async Task<IList<recordType>> GetRecordsListAsync(string format, bool fetchCompleteList)
        {
            var records = await GetVerbAsync<ListRecordsType>(verbType.ListRecords, format);
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

        public async Task<IList<recordType>> GetRecordsListAsync(string format, string set, bool fetchCompleteList)
        {
            var records = await GetVerbAsync<ListRecordsType>(verbType.ListRecords, format, set);
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


        public async Task<ListRecordsType> GetRecordsListAsync(string resumptionToken)
        {
            return await GetResumptionTokenAsync<ListRecordsType>(verbType.ListIdentifiers, resumptionToken);
        }

        public async Task<OaiPmhResourceInfo> IdentifyAsync()
        {
            var identify = await GetVerbAsync<IdentifyType>(verbType.Identify);

            return new OaiPmhResourceInfo
            {
                AdminMails = identify.adminEmail,
                Description = identify.description?.ToString(),
                Url = identify.baseURL,
                EarliestDateTime = DateTime.Parse(identify.earliestDatestamp),
                Granularity = identify.granularity.ToString(),
                Name = identify.repositoryName
            };
        }

        public async Task<recordType> GetRecordAsync(string format, string identifier)
        {
            var responseOai = await GetResponseAsync(Verb + verbType.GetRecord + MetadataPrefix + format + "&identifier=" + identifier);
            var record = (GetRecordType) responseOai.Items.First();
            return record.record;
        }

        private async Task<OAIPMHType> GetResponseAsync(string query)
        {
            var currentRetry = 0;

            for (;;)
            {
                try
                {
                    var streamResult = await m_httpClient.GetStreamAsync(Url + query);
                    var oaiPmhRecordResponse = (OAIPMHType)m_oaiPmhXmlSerializer.Deserialize(streamResult);

                    //Validate response
                    if (oaiPmhRecordResponse.Items.First().GetType() == typeof(OAIPMHerrorType))
                    {
                        var error = (OAIPMHerrorType) oaiPmhRecordResponse.Items.First();
                        throw new OaiPmhException(error.Value, error.code);
                    }

                    return oaiPmhRecordResponse;
                }
                catch (HttpRequestException)
                {
                    currentRetry++;

                    if (currentRetry > m_retryCount)
                    {
                        throw;
                    }
                }

                await Task.Delay(m_delay); 
            }
        }
      
        public async Task<T> GetVerbAsync<T>(verbType verbType, string format = null, string set = null)
        {
            return (T) (await GetResponseAsync(Verb + verbType + (string.IsNullOrEmpty(format) ? "" : MetadataPrefix + format) + (string.IsNullOrEmpty(set) ? "" : Set + set))).Items.First();
        }

        private async Task<T> GetResumptionTokenAsync<T>(verbType verbType, string resumptionToken)
        {
            return (T)(await GetResponseAsync(Verb + verbType + ResumptionToken + resumptionToken)).Items.First();
        }

        public void Dispose()
        {
            m_httpClient?.Dispose();
        }
    }
}