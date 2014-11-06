using Castle.Windsor;

namespace ITJakub.FileProcessing.Service
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly WindsorContainer m_container;
        private readonly FileProcessingManager m_fileProcessingManager;

        public FileProcessingService()
        {
            m_container = Container.Current;
            m_fileProcessingManager = m_container.Resolve<FileProcessingManager>();
        }

        public void TestXml()
        {
            m_fileProcessingManager.TestXml();
        }
    }
}