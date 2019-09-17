using System;
using System.IO;
using Vokabular.Core.Storage;
using Vokabular.Core.Storage.Resources;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Test.Mock
{
    public class MockFileSystemManager : IFileSystemManager
    {
        public SaveResourceResult FileInfo { get; set; }

        public Stream GetResource(long projectId, string bookVersionId, string fileName, ResourceType resourceType)
        {
            throw new InvalidOperationException("This operation is currently not supported in this Mock.");
        }

        public void SaveResource(long projectId, string bookVersionId, FileResource resource)
        {
        }

        public SaveResourceResult SaveResource(ResourceType resourceType, long projectId, Stream dataStream, string bookVersionId = null,
            string fileName = null)
        {
            return FileInfo;
        }
    }
}
