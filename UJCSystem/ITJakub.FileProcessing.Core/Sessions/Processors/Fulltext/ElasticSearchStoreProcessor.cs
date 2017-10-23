using System;
using System.Collections.Generic;
using ITJakub.FileProcessing.Core.Communication;
using ITJakub.Shared.Contracts;

namespace ITJakub.FileProcessing.Core.Sessions.Processors.Fulltext
{
    public class ElasticSearchStoreProcessor : IFulltextResourceProcessor
    {
        private readonly FileProcessingCommunicationProvider m_communicationProvider;

        public ElasticSearchStoreProcessor(FileProcessingCommunicationProvider communicationProvider)
        {
            m_communicationProvider = communicationProvider;
        }

        public void UploadFullbookToBookVersion(VersionResourceUploadContract resourceUploadContract)
        {
            // This data is not stored in ElasticSearch
        }

        public string UploadPageToBookVersion(VersionResourceUploadContract resourceUploadContract)
        {
            using (var client = m_communicationProvider.GetFulltextServiceClient())
            {
                // TODO convert stream content from XML to TXT
                throw new NotImplementedException();
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

        public void PublishSnapshot(long projectId, List<string> externalPageIds)
        {
            // TODO is projectId required, is snapshotId required?
            throw new NotImplementedException();
        }
    }
}