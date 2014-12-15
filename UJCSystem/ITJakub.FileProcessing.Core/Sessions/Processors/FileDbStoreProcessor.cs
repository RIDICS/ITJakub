using ITJakub.Core;
using ITJakub.Core.Resources;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class FileDbStoreProcessor : IResourceProcessor
    {
        private readonly FileSystemManager m_fileSystemManager;

        public FileDbStoreProcessor(FileSystemManager fileSystemManager)
        {
            m_fileSystemManager = fileSystemManager;
        }

        public void Process(ResourceSessionDirector resourceSessionDirector)
        {
            foreach (Resource resource in resourceSessionDirector.Resources)
            {
                m_fileSystemManager.SaveResource(
                    resourceSessionDirector.GetSessionInfoValue<string>(SessionInfo.BookId),
                    resourceSessionDirector.GetSessionInfoValue<string>(SessionInfo.VersionId), resource);
            }
        }
    }
}