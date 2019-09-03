using System.Collections.Generic;
using ITJakub.FileProcessing.DataContracts;
using ITJakub.SearchService.DataContracts.Contracts;
using Vokabular.DataEntities.Database.Entities;

namespace ITJakub.FileProcessing.Core.Sessions.Processors.Fulltext
{
    public interface IFulltextResourceProcessor
    {
        FulltextStoreTypeContract StoreType { get; }

        void UploadFullbookToBookVersion(VersionResourceUploadContract resourceUploadContract);

        string UploadPageToBookVersion(VersionResourceUploadContract resourceUploadContract);

        void UploadTransformationResource(VersionResourceUploadContract resourceUploadContract);

        void UploadBibliographyFile(VersionResourceUploadContract resourceUploadContract);

        void PublishSnapshot(long snapshotId, long projectId, List<string> externalPageIds, MetadataResource metadata);
    }
}
