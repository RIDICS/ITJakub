using System.Threading.Tasks;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.Sessions.Processors;

namespace ITJakub.FileProcessing.Core.Sessions
{
    public class ResourceProcessorManager
    {
        private readonly XmlConversionProcessor m_xmlConversionProcessor;
        private readonly MetadataProcessor m_metadataProcessor;
        private readonly RelationalDbStoreProcessor m_relationalDbStoreProcessor;
        private readonly FileDbStoreProcessor m_fileDbStoreProcessor;
        private readonly ExistDbStoreProcessor m_existDbStoreProcessor;


        public ResourceProcessorManager(XmlConversionProcessor xmlConversionProcessor, MetadataProcessor metadataProcessor, RelationalDbStoreProcessor relationalDbStoreProcessor, FileDbStoreProcessor fileDbStoreProcessor, ExistDbStoreProcessor existDbStoreProcessor )
        {
            m_xmlConversionProcessor = xmlConversionProcessor;
            m_metadataProcessor = metadataProcessor;
            m_relationalDbStoreProcessor = relationalDbStoreProcessor;
            m_fileDbStoreProcessor = fileDbStoreProcessor;
            m_existDbStoreProcessor = existDbStoreProcessor;
        }

        public bool ProcessSessionResources(ResourceSessionDirector resourceDirector)
        {
            ProcessXmlConversion(resourceDirector); //call of library to convert docx to xml resources which are added to resources in paramater
            var bookVersion = ProcessMetaData(resourceDirector); //obtain entity information from processing metadata
            var existTask = Task.Factory.StartNew(() => ProcessExistDbStore(bookVersion.Book.Guid, bookVersion.VersionId, resourceDirector));   //saves xmls to Exist
            var resourceTask = Task.Factory.StartNew(() => ProcessFileDbStore(bookVersion.Book.Guid, bookVersion.VersionId, resourceDirector));  //saves images, docx etc on physical disk
            Task.WaitAll(new[] {existTask, resourceTask});
            ProcessRelationalDbStore(bookVersion); //if everything was ok then saves entity into relational database
            //TODO add try catch with rollback and return false
            return true;
        }

        private void ProcessFileDbStore(string bookId, string versionId, ResourceSessionDirector resourceDirector)
        {
            m_fileDbStoreProcessor.Process(bookId, versionId, resourceDirector.Resources);
        }

        private void ProcessExistDbStore(string bookId, string versionId, ResourceSessionDirector resourceDirector)
        {
            m_existDbStoreProcessor.Process(bookId, versionId, resourceDirector.Resources);
        }

        private void ProcessRelationalDbStore(BookVersion bookVersion)
        {
            m_relationalDbStoreProcessor.Process(bookVersion);
        }

        private BookVersion ProcessMetaData(ResourceSessionDirector resourceDirector)
        {
            return m_metadataProcessor.Process(resourceDirector.Resources);
        }

        private void ProcessXmlConversion(ResourceSessionDirector resourceDirector)
        {
            m_xmlConversionProcessor.Process(resourceDirector.Resources);
        }

    }
}