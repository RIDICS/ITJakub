using Vokabular.Core.Storage;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class FileDbStoreProcessor : IResourceProcessor
    {
        private readonly IFileSystemManager m_fileSystemManager;

        public FileDbStoreProcessor(IFileSystemManager fileSystemManager)
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