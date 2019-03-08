using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Vokabular.ProjectImport.ImportManagers;
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

        public async Task ImportFromResource(string configuration, ITargetBlock<string> buffer, CancellationToken cancellationToken = default(CancellationToken))
        {
            var oaiPmhResource = JsonConvert.DeserializeObject<OaiPmhRepositoryConfiguration>(configuration);

            using (var client = GetOaiPmhCommunicationClient(oaiPmhResource.Url))
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
                        //TODO FIXXX
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

        public Project ImportRecord(string configuration, string id)
        {
            throw new NotImplementedException();
        }
        /*public override Project ImportRecord(Resource resource, string id)
        {
            var oaiPmhResource = JsonConvert.DeserializeObject<OaiPmhRepositoryConfiguration>(resource.Configuration);
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
