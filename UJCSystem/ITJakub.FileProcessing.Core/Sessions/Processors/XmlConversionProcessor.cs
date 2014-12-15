using System.Collections.Generic;
using System.IO;
using System.Linq;
using ITJakub.Core.Resources;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts.Resources;
using Ujc.Ovj.Ooxml.Conversion;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class XmlConversionProcessor : IResourceProcessor
    {
        private readonly BookRepository m_bookRepository;

        public XmlConversionProcessor(BookRepository bookRepository)
        {
            m_bookRepository = bookRepository;
        }


        public void Process(ResourceSessionDirector resourceSessionDirector)
        {
            Resource inputFileResource =
                resourceSessionDirector.Resources.First(
                    resource => resource.ResourceType == ResourceTypeEnum.SourceDocument);

            string metaDataFileName = string.Format("{0}_metadata.xml",
                Path.GetFileNameWithoutExtension(inputFileResource.FileName));
            var metaDataResource = new Resource
            {
                FileName = metaDataFileName,
                FullPath = Path.Combine(resourceSessionDirector.SessionPath, metaDataFileName),
                ResourceType = ResourceTypeEnum.Metadata
            };

            string bookFileName = string.Format("{0}.xml", Path.GetFileNameWithoutExtension(inputFileResource.FileName));
            var bookResource = new Resource
            {
                FileName = bookFileName,
                FullPath = Path.Combine(resourceSessionDirector.SessionPath, bookFileName),
                ResourceType = ResourceTypeEnum.Book
            };

            string tmpDirPath = Path.Combine(resourceSessionDirector.SessionPath, "tmp");
            if (!Directory.Exists(tmpDirPath))
            {
                Directory.CreateDirectory(tmpDirPath);
            }

            var settings = new DocxToTeiConverterSettings
            {
                Debug = false,
                InputFilePath = inputFileResource.FullPath,
                //Message = resourceSessionDirector.GetSessionInfoValue<string>(SessionInfo.Message), //TODO fill in client and pass it
                Message = "ahoj",
                MetadataFilePath = "D:\\Pool\\xmlconversion\\Ujc.Ovj.Ooxml.Conversion.Test\\Data\\Input\\Evidence.xml",//TODO load from config path to evidence.xml
                OutputFilePath = bookResource.FullPath,
                TempDirectoryPath = tmpDirPath
            };

            var converter = new DocxToTeiConverter();
            converter.Convert(settings);

            resourceSessionDirector.Resources.Add(metaDataResource);
            resourceSessionDirector.Resources.Add(bookResource);
        }

        private List<Version> GetVersionsByBookId(string bookId)
        {
            IEnumerable<BookVersion> versions = m_bookRepository.GetAllVersionsByBookId(bookId);
            return versions.Select(x => new Version(x.Description, x.CreateTime)).ToList();
        }
    }
}