using System.Collections.Generic;
using ITJakub.FileProcessing.Core.Communication;
using ITJakub.FileProcessing.DataContracts;
using ITJakub.SearchService.DataContracts.Contracts;
using Vokabular.DataEntities.Database.Entities;

namespace ITJakub.FileProcessing.Core.Sessions.Processors.Fulltext
{
    public class ExistDbStoreProcessor : IFulltextResourceProcessor
    {
        private readonly FileProcessingCommunicationProvider m_communicationProvider;

        public ExistDbStoreProcessor(FileProcessingCommunicationProvider communicationProvider)
        {
            m_communicationProvider = communicationProvider;
        }

        public FulltextStoreTypeContract StoreType => FulltextStoreTypeContract.ExistDb;

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

        public void PublishSnapshot(long snapshotId, long projectId, List<string> externalPageIds, MetadataResource metadata)
        {
            // Snapshots are not supported in eXistDB storage, uploaded book version is directly published
        }
    }
}