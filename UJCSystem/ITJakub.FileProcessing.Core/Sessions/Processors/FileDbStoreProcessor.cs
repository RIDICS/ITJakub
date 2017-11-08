using Vokabular.Core.Storage;

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
            var projectId = resourceSessionDirector.GetSessionInfoValue<long>(SessionInfo.ProjectId);
            var versionXmlId = resourceSessionDirector.GetSessionInfoValue<string>(SessionInfo.VersionXmlId);

            foreach (var resource in resourceSessionDirector.Resources)
            {
                m_fileSystemManager.SaveResource(projectId, versionXmlId, resource);
            }
        }
    }
}