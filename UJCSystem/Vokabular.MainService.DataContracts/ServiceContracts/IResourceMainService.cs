using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.DataContracts.ServiceContracts
{
    public interface IResourceMainService
    {
        void DeleteResource(long resourceId);
        long DuplicateResource(long resourceId);
        List<ResourceContract> GetResourceList(long projectId, ResourceTypeContract? resourceType);
        List<ResourceVersionContract> GetResourceVersionHistory(long resourceId);
        ResourceMetadataContract GetResourceMetadata(long resourceId);
        long ProcessUploadedResources(long projectId, NewResourceContract resourceInfo);
        long ProcessUploadedResourceVersion(long resourceId, NewResourceContract resourceInfo);
        void RenameResource(long resourceId, ResourceContract resource);
        void UploadResource(string sessionId);
    }
}