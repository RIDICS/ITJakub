using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;
using Vokabular.OaiPmhImportManager.Model;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectImport.Shared.Const;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.OaiPmhImportManager
{
    public class OaiPmhProjectImportManager : IProjectImportManager
    {
        private readonly OaiPmhCommunicationFactory m_oaiPmhCommunicationFactory;

        public OaiPmhProjectImportManager(OaiPmhCommunicationFactory oaiPmhCommunicationFactory)
        {
            m_oaiPmhCommunicationFactory = oaiPmhCommunicationFactory;
        }

        public string ExternalRepositoryTypeName { get; } = ExternalRepositoryTypeNameConstant.OaiPhm;

        public virtual async Task ImportFromResource(string configuration, ITargetBlock<object> buffer, RepositoryImportProgressInfo progressInfo,
            DateTime? lastImport = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var oaiPmhResource = JsonConvert.DeserializeObject<OaiPmhRepositoryConfiguration>(configuration);

            using (var client = m_oaiPmhCommunicationFactory.CreateClient(oaiPmhResource.Url))
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
                    records = await client.GetRecordsListWithRetryAsync(resumptionToken.Value);
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
            projectImportMetadata.IsDeleted = record.header.statusSpecified;

            if (!string.IsNullOrEmpty(record.header.datestamp))
            {
                try
                {
                    projectImportMetadata.TimeStamp =  DateTime.Parse(record.header.datestamp);
                }
                catch (ArgumentOutOfRangeException)
                {
                    //The date is set to zero - we are omitting this information
                }
            }
            
            return projectImportMetadata;
        }
    }
}