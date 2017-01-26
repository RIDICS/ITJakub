using ITJakub.Core;

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
            foreach (var resource in resourceSessionDirector.Resources)
            {
                m_fileSystemManager.SaveResource(
                    resourceSessionDirector.GetSessionInfoValue<string>(SessionInfo.BookXmlId),
                    resourceSessionDirector.GetSessionInfoValue<string>(SessionInfo.VersionXmlId), resource);
            }
        }
    }
}