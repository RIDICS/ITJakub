using System.Collections.Generic;
using ITJakub.FileProcessing.Core.Communication;
using ITJakub.SearchService.DataContracts.Contracts;
using Vokabular.Shared.Converters;

namespace ITJakub.FileProcessing.Core.Sessions.Processors.Fulltext
{
    public class ElasticSearchStoreProcessor : IFulltextResourceProcessor
    {
        private readonly FileProcessingCommunicationProvider m_communicationProvider;
        private readonly IXmlToTextConverter m_converter;


        public ElasticSearchStoreProcessor(FileProcessingCommunicationProvider communicationProvider, IXmlToTextConverter converter)
        {
            m_communicationProvider = communicationProvider;
            m_converter = converter;
        }

        public void UploadFullbookToBookVersion(VersionResourceUploadContract resourceUploadContract)
        {
            // This data is not stored in ElasticSearch
        }

        public string UploadPageToBookVersion(VersionResourceUploadContract resourceUploadContract)
        {
            using (var client = m_communicationProvider.GetFulltextServiceClient())
            {
                return client.CreateTextResource(m_converter.Convert(resourceUploadContract.DataStream), 1);
            }
        }

        public void UploadTransformationResource(VersionResourceUploadContract resourceUploadContract)
        {
            // This data is not stored in ElasticSearch
        }

        public void UploadBibliographyFile(VersionResourceUploadContract resourceUploadContract)
        {
            // This data is not stored in ElasticSearch
        }

        public void PublishSnapshot(long snapshotId, long projectId, List<string> externalPageIds)
        {
            using (var client = m_communicationProvider.GetFulltextServiceClient())
            {
                client.CreateSnapshot(snapshotId, projectId, externalPageIds);
            }
        }
    }
}