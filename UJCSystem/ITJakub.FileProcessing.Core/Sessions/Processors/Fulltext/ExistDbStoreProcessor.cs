﻿using ITJakub.FileProcessing.Core.Communication;
using ITJakub.Shared.Contracts;

namespace ITJakub.FileProcessing.Core.Sessions.Processors.Fulltext
{
    public class ExistDbStoreProcessor : IFulltextResourceProcessor
    {
        private readonly FileProcessingCommunicationProvider m_communicationProvider;

        public ExistDbStoreProcessor(FileProcessingCommunicationProvider communicationProvider)
        {
            m_communicationProvider = communicationProvider;
        }

        public void UploadFullbookToBookVersion(VersionResourceUploadContract resourceUploadContract)
        {
            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                ssc.UploadVersionFile(resourceUploadContract);
            }
        }

        public string UploadPageToBookVersion(VersionResourceUploadContract resourceUploadContract)
        {
            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                ssc.UploadVersionFile(resourceUploadContract);
                return null; // correct ID is already filled in metadata
            }
        }

        public void UploadTransformationResource(VersionResourceUploadContract resourceUploadContract)
        {
            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                ssc.UploadBookFile(resourceUploadContract);
            }
        }

        public void UploadBibliographyFile(VersionResourceUploadContract resourceUploadContract)
        {
            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                ssc.UploadBibliographyFile(resourceUploadContract);
            }
        }
    }
}