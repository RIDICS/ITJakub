using System.Collections.Generic;
using ITJakub.FileStorage;
using ITJakub.FileStorage.Resources;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class FileDbStoreProcessor
    {
        private readonly FileSystemManager m_fileSystemManager;
            
        public FileDbStoreProcessor(FileSystemManager fileSystemManager)
        {
            m_fileSystemManager = fileSystemManager;
        }

        public void Process(string bookId, string versionId, IEnumerable<Resource> resources)
        {
            foreach (var resource in resources)
            {
                m_fileSystemManager.SaveResource(bookId, versionId, resource);
            }
        }
    }
}