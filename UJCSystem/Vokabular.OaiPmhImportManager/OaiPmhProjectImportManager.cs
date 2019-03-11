using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Vokabular.ProjectImport.Model;
using Vokabular.Shared.Options;
using Project = Vokabular.ProjectParsing.Model.Entities.Project;
using ProjectImportMetadata = Vokabular.ProjectParsing.Model.Entities.ProjectImportMetadata;

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

        public async Task ImportFromResource(string configuration, ITargetBlock<object> buffer, RepositoryImportProgressInfo progressInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            var oaiPmhResource = JsonConvert.DeserializeObject<OaiPmhRepositoryConfiguration>(configuration);

            using (var client = GetOaiPmhCommunicationClient(oaiPmhResource.Url))
            {
                var records = await client.GetVerbAsync<ListRecordsType>(verbType.ListRecords, oaiPmhResource.DataFormat, oaiPmhResource.SetName);
                var resumptionToken = records.resumptionToken;
                progressInfo.TotalProjectsCount = Convert.ToInt32(resumptionToken.completeListSize);
               
                foreach (var recordType in records.record)
                {
                    buffer.Post(recordType);
                }

                while (resumptionToken != null && !cancellationToken.IsCancellationRequested)
                {
                    records = await client.GetVerbAsync<ListRecordsType>(verbType.ListRecords, oaiPmhResource.DataFormat, oaiPmhResource.SetName);
                    resumptionToken = records.resumptionToken;

                    foreach (var recordType in records.record)
                    {
                        buffer.Post(recordType);
                    }
                }
            }
        }

        public ProjectImportMetadata ParseResponse(object response)
        {
            var projectImportMetadata = new ProjectImportMetadata();
            try
            {
                var record = (recordType) response;
                projectImportMetadata.ExternalId = record.header.identifier;
                projectImportMetadata.RawData = record.metadata.OuterXml;
                
            }
            catch (Exception e)
            {
                projectImportMetadata.IsFaulted = true;
                projectImportMetadata.FaultedMessage = e.Message;
            }

            return projectImportMetadata;
        }

        public Project ImportRecord(string configuration, string id)
        {
            throw new NotImplementedException();
        }
    }
}
