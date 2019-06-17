using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vokabular.OaiPmhImportManager.Model;
using Vokabular.ProjectImport.Model.Exceptions;
using Vokabular.ProjectImport.Shared.Options;

namespace Vokabular.OaiPmhImportManager
{
    public class OaiPmhCommunicationClientWrapper : OaiPmhCommunicationClient
    {
        private readonly int m_retryCount;
        private readonly int m_delay;
        private readonly string m_url;
        private readonly ILogger<OaiPmhCommunicationClientWrapper> m_logger;

        public OaiPmhCommunicationClientWrapper(OaiPmhClientOption option, string url, ILogger<OaiPmhCommunicationClientWrapper> logger) : base(url, option.DisableSslValidation)
        {
            m_retryCount = option.RetryCount;
            m_delay = option.Delay;
            m_url = url;
            m_logger = logger;
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

        public async Task<ListRecordsType> GetRecordsListWithRetryAsync(string resumptionToken)
        {
            var currentRetry = 0;

            for (;;)
            {
                try
                {
                    return await GetResumptionTokenAsync<ListRecordsType>(verbType.ListRecords, resumptionToken);
                }
                catch (HttpRequestException e)
                {
                    m_logger.LogWarning($"Error while requesting: {m_url}, verb: {verbType.ListRecords}, resumptionToken: {resumptionToken}. Error message: {e.Message}. Current retry: {currentRetry}. Wait {m_delay * Math.Pow(2, currentRetry) / 1000} seconds.", e);

                    await Task.Delay((int)(m_delay * Math.Pow(2, currentRetry)));
                    currentRetry++;

                    if (currentRetry > m_retryCount)
                    {
                        throw new ImportFailedException($"Error while requesting: {m_url}, verb: {verbType.ListRecords}, resumptionToken: {resumptionToken}. Error message: {e.Message}", e);
                    }
                }
            }
        }
    }
}