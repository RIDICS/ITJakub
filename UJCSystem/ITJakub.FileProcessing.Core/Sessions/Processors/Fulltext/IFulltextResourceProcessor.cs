using System.Collections.Generic;
using ITJakub.Shared.Contracts;

namespace ITJakub.FileProcessing.Core.Sessions.Processors.Fulltext
{
    public interface IFulltextResourceProcessor
    {
        void UploadFullbookToBookVersion(VersionResourceUploadContract resourceUploadContract);

        string UploadPageToBookVersion(VersionResourceUploadContract resourceUploadContract);

        void UploadTransformationResource(VersionResourceUploadContract resourceUploadContract);

        void UploadBibliographyFile(VersionResourceUploadContract resourceUploadContract);

        void PublishSnapshot(long projectId, List<string> externalPageIds);
    }
}
