using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;
using Vokabular.CommunicationService;
using Vokabular.CommunicationService.OAIPMH;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.ProjectImport.Model;
using Project = Vokabular.ProjectParsing.Model.Entities.Project;
using ProjectImportMetadata = Vokabular.ProjectParsing.Model.Entities.ProjectImportMetadata;

namespace Vokabular.ProjectImport.ImportManagers
{
    public class OaiPmhProjectImportManager : IProjectImportManager
    {
        public string ExternalRepositoryTypeName { get; } = "OaiPmh";
        private readonly CommunicationProvider m_communicationProvider;

        public OaiPmhProjectImportManager(CommunicationProvider communicationProvider)
        {
            m_communicationProvider = communicationProvider;
        }

        public async Task ImportFromResource(ExternalRepository repository, ITargetBlock<string> buffer, CancellationToken cancellationToken = default(CancellationToken))
        {
            var oaiPmhResource = JsonConvert.DeserializeObject<OaiPmhResource>(repository.Configuration);

            using (var client = m_communicationProvider.GetOaiPmhCommunicationClient(oaiPmhResource.Url))
            {
                var records = await client.GetVerbAsync<ListRecordsType>(verbType.ListRecords, oaiPmhResource.DataFormat, oaiPmhResource.SetName);
                var resumptionToken = records.resumptionToken;
               
                foreach (var recordType in records.record)
                {
                    buffer.Post(recordType.metadata.OuterXml);
                }

                var testCount = 1; //TODO remove

                while (resumptionToken != null && !cancellationToken.IsCancellationRequested)
                {
                    records = await client.GetVerbAsync<ListRecordsType>(verbType.ListRecords, oaiPmhResource.DataFormat, oaiPmhResource.SetName);
                    resumptionToken = records.resumptionToken;

                    foreach (var recordType in records.record)
                    {
                        //TODO post recordType or special object
                        buffer.Post(recordType.metadata.OuterXml);
                    }

                    testCount++; //TODO remove
                    if (testCount == 100)
                    {
                        break;
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

        public Project ImportRecord(ExternalRepository repository, string id)
        {
            throw new NotImplementedException();
        }
        /*public override Project ImportRecord(Resource resource, string id)
        {
            var oaiPmhResource = JsonConvert.DeserializeObject<OaiPmhResource>(resource.Configuration);
            var config = new Dictionary<ParserHelperTypes, string>
                {{ParserHelperTypes.TemplateUrl, oaiPmhResource.TemplateUrl}};

            using (var client = m_communicationProvider.GetOaiPmhCommunicationClient(resource))
            {
                var record = client.GetRecord(oaiPmhResource.DataFormat, id);

                m_parsers.TryGetValue(resource.BibliographicFormat, out var parser);
                var result = parser?.Parse(record.metadata.OuterXml, config);

                return result;
            }
        }*/
    }
}
