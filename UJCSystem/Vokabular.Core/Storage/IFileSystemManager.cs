using System.IO;
using Vokabular.Core.Storage.Resources;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Core.Storage
{
    public interface IFileSystemManager
    {
        Stream GetResource(long projectId, string bookVersionId, string fileName, ResourceType resourceType);
        void SaveResource(long projectId, string bookVersionId, FileResource resource);
        SaveResourceResult SaveResource(ResourceType resourceType, long projectId, Stream dataStream, string bookVersionId = null, string fileName = null);
    }
}