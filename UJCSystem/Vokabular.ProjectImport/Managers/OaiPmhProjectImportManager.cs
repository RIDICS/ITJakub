using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;
using Vokabular.CommunicationService;
using Vokabular.CommunicationService.OAIPMH;
using Vokabular.ProjectImport.DataEntities.Database;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectImport.Managers
{
    public class OaiPmhProjectImportManager : ProjectImportManagerBase
    {
        private readonly CommunicationProvider m_communicationProvider;

        public OaiPmhProjectImportManager(CommunicationProvider communicationProvider) : base(ResourceType.Oaipmh)
        {
            m_communicationProvider = communicationProvider;
        }

        public override async Task ImportFromResource(Resource resource, ITargetBlock<string> buffer, CancellationToken cancellationToken = default(CancellationToken))
        {
            var oaiPmhResource = JsonConvert.DeserializeObject<OaiPmhResource>(resource.Configuration);

            //TODO where to set ID? 
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

        public override Project ImportRecord(Resource resource, string id)
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

                m_parsers.TryGetValue(resource.ParserType, out var parser);
                var result = parser?.Parse(record.metadata.OuterXml, config);

                return result;
            }
        }*/
    }
}
