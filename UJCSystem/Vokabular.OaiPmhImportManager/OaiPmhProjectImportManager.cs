using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Vokabular.OaiPmhImportManager.Model;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.Shared.Options;

namespace Vokabular.OaiPmhImportManager
{
    public class OaiPmhProjectImportManager : IProjectImportManager
    {
        private readonly OaiPmhClientOption m_oaiPmhClientOption;

        public OaiPmhProjectImportManager(IOptions<OaiPmhClientOption> oaiPmhOptions)
        {
            m_oaiPmhClientOption = oaiPmhOptions.Value;
        }

        public string ExternalRepositoryTypeName { get; } = "OaiPmh";

        public OaiPmhCommunicationClient GetOaiPmhCommunicationClient(string url)
        {
            return new OaiPmhCommunicationClient(m_oaiPmhClientOption, url);
        }

        public async Task ImportFromResource(string configuration, ITargetBlock<object> buffer, RepositoryImportProgressInfo progressInfo,
            DateTime? lastImport = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var oaiPmhResource = JsonConvert.DeserializeObject<OaiPmhRepositoryConfiguration>(configuration);

            using (var client = GetOaiPmhCommunicationClient(oaiPmhResource.Url))
            {
                var records = await client.GetRecordsListAsync(oaiPmhResource.DataFormat, oaiPmhResource.SetName, lastImport);
                var resumptionToken = records.resumptionToken;
                progressInfo.TotalProjectsCount = Convert.ToInt32(resumptionToken.completeListSize);

                foreach (var recordType in records.record)
                {
                    buffer.Post(recordType);
                }

                while (resumptionToken.Value != null && !cancellationToken.IsCancellationRequested)
                {
                    records = await client.GetRecordsListAsync(resumptionToken.Value);
                    resumptionToken = records.resumptionToken;

                    foreach (var recordType in records.record)
                    {
                        buffer.Post(recordType);
                    }
                }
            }
        }

        public ImportedRecord ParseResponse(object response)
        {
            var projectImportMetadata = new ImportedRecord();

            var record = (recordType) response;
            projectImportMetadata.ExternalId = record.header.identifier;
            projectImportMetadata.RawData = record.metadata.OuterXml;
            projectImportMetadata.IsDeleted = record.header.status == statusType.deleted;
            
            return projectImportMetadata;
        }
    }
}