using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ITJakub.FileProcessing.Core.Sessions.Processors;
using log4net;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.FileProcessing.Core.Sessions
{
    public class ResourceProcessorManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly AudioBookArchiveProcessor m_audiobookArchiveProcessor;
        private readonly FulltextDbStoreProcessor m_fulltextDbStoreProcessor;
        private readonly BasicProjectDataRelationalDbStoreProcessor m_basicProjectDataRelationalDbStoreProcessor;
        private readonly ExtractableArchiveProcessor m_extractableArchiveProcessor;
        private readonly FileDbStoreProcessor m_fileDbStoreProcessor;
        private readonly MetadataProcessor m_metadataProcessor;
        private readonly RelationalDbStoreProcessor m_relationalDbStoreProcessor;
        private readonly TransformationsProcessor m_transformationsProcessor;
        private readonly XmlConversionProcessor m_xmlConversionProcessor;

        public ResourceProcessorManager(XmlConversionProcessor xmlConversionProcessor,
            MetadataProcessor metadataProcessor, RelationalDbStoreProcessor relationalDbStoreProcessor,
            FileDbStoreProcessor fileDbStoreProcessor, FulltextDbStoreProcessor fulltextDbStoreProcessor,
            ExtractableArchiveProcessor extractableArchiveProcessor, TransformationsProcessor transformationsProcessor,
            AudioBookArchiveProcessor audiobookArchiveProcessor,
            BasicProjectDataRelationalDbStoreProcessor basicProjectDataRelationalDbStoreProcessor)
        {
            m_xmlConversionProcessor = xmlConversionProcessor;
            m_metadataProcessor = metadataProcessor;
            m_relationalDbStoreProcessor = relationalDbStoreProcessor;
            m_fileDbStoreProcessor = fileDbStoreProcessor;
            m_fulltextDbStoreProcessor = fulltextDbStoreProcessor;
            m_extractableArchiveProcessor = extractableArchiveProcessor;
            m_transformationsProcessor = transformationsProcessor;
            m_audiobookArchiveProcessor = audiobookArchiveProcessor;
            m_basicProjectDataRelationalDbStoreProcessor = basicProjectDataRelationalDbStoreProcessor;
        }

        public bool ProcessSessionResources(ResourceSessionDirector resourceDirector)
        {
            if (resourceDirector.Resources.Any(x => x.ResourceType == ResourceType.ExtractableArchive))
                ProcessExtractableArchive(resourceDirector);

            if (resourceDirector.Resources.Any(x => x.ResourceType == ResourceType.SourceDocument))
                ProcessXmlConversion(resourceDirector); //call of library to convert docx to xml resources which are added to resources in paramater

            ProcessMetaData(resourceDirector); //obtain entity information from processing metadata

            ProcessTransformations(resourceDirector); //Process Transformations

            if (resourceDirector.Resources.Any(x => x.ResourceType == ResourceType.Audio))
                GenerateFullBooksResources(resourceDirector); //GenerateFull audio records

            ProcessBasicProjectDataRelationalDbStore(resourceDirector); //update or generate new Project in relational database

            var existTask = Task.Factory.StartNew(() => ProcessExistDbStore(resourceDirector)); //saves xmls to Exist
            var resourceTask = Task.Factory.StartNew(() => ProcessFileDbStore(resourceDirector)); //saves images, docx etc on physical disk
            Task.WaitAll(existTask, resourceTask);

            ProcessRelationalDbStore(resourceDirector); //if everything was ok then saves entity into relational database
            //TODO add try catch with rollback and return false
            return true;
        }

        private void GenerateFullBooksResources(ResourceSessionDirector resourceDirector)
        {
            m_audiobookArchiveProcessor.Process(resourceDirector);
        }

        private void ProcessTransformations(ResourceSessionDirector resourceDirector)
        {
            m_transformationsProcessor.Process(resourceDirector);
        }

        private void ProcessExtractableArchive(ResourceSessionDirector resourceDirector)
        {
            m_extractableArchiveProcessor.Process(resourceDirector);
        }

        private void ProcessFileDbStore(ResourceSessionDirector resourceDirector)
        {
            try
            {
                m_fileDbStoreProcessor.Process(resourceDirector);
            }
            catch (Exception e)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error("Error storing data to file storage", e);
                throw;
            }
        }

        private void ProcessExistDbStore(ResourceSessionDirector resourceDirector)
        {
            try
            {
                m_fulltextDbStoreProcessor.Process(resourceDirector);
            }
            catch (Exception e)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error("Error storing data to fulltext database", e);
                throw;
            }
        }

        private void ProcessBasicProjectDataRelationalDbStore(ResourceSessionDirector resourceDirector)
        {
            try
            {
                m_basicProjectDataRelationalDbStoreProcessor.Process(resourceDirector);
            }
            catch (Exception e)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error("Error storing basic project data to relational database", e);
                throw;
            }
        }

        private void ProcessRelationalDbStore(ResourceSessionDirector resourceDirector)
        {
            try
            {
                m_relationalDbStoreProcessor.Process(resourceDirector);
            }
            catch (Exception e)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error("Error storing data to relational database", e);
                throw;
            }
        }

        private void ProcessMetaData(ResourceSessionDirector resourceDirector)
        {
            try
            {
                m_metadataProcessor.Process(resourceDirector);
            }
            catch (Exception e)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error("Error processing XMD metadata", e);
                throw;
            }
        }

        private void ProcessXmlConversion(ResourceSessionDirector resourceDirector)
        {
            try
            {
                m_xmlConversionProcessor.Process(resourceDirector);
            }
            catch (Exception e)
            {
                if (m_log.IsErrorEnabled)
                    m_log.Error("Error processing DOCX file to XML", e);
                throw;
            }
        }
    }
}