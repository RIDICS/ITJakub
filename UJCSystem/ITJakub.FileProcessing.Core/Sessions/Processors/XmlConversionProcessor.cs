using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ITJakub.Core.Resources;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts.Resources;
using Ujc.Ovj.Ooxml.Conversion;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class XmlConversionProcessor : IResourceProcessor
    {
        private readonly BookRepository m_bookRepository;
        private readonly string m_conversionMetadataPath;
        private readonly VersionIdGenerator m_versionIdGenerator;

        public XmlConversionProcessor(string conversionMetadataPath, BookRepository bookRepository, VersionIdGenerator versionIdGenerator)
        {
            m_conversionMetadataPath = conversionMetadataPath;
            m_bookRepository = bookRepository;
            m_versionIdGenerator = versionIdGenerator;
        }

        public void Process(ResourceSessionDirector resourceSessionDirector)
        {
            var inputFileResource =
                resourceSessionDirector.Resources.First(
                    resource => resource.ResourceType == ResourceType.SourceDocument);

            var metaDataFileName = string.Format("{0}.xmd",
                Path.GetFileNameWithoutExtension(inputFileResource.FileName));
            var metaDataResource = new Resource
            {
                FileName = metaDataFileName,
                FullPath = Path.Combine(resourceSessionDirector.SessionPath, metaDataFileName),
                ResourceType = ResourceType.Metadata
            };

            var bookFileName = string.Format("{0}.xml", Path.GetFileNameWithoutExtension(inputFileResource.FileName));
            var bookResource = new Resource
            {
                FileName = bookFileName,
                FullPath = Path.Combine(resourceSessionDirector.SessionPath, bookFileName),
                ResourceType = ResourceType.Book
            };

            var tmpDirPath = Path.Combine(resourceSessionDirector.SessionPath, "tmp");
            if (!Directory.Exists(tmpDirPath))
            {
                Directory.CreateDirectory(tmpDirPath);
            }

            var message = resourceSessionDirector.GetSessionInfoValue<string>(SessionInfo.Message);
            var createTime = resourceSessionDirector.GetSessionInfoValue<DateTime>(SessionInfo.CreateTime);

            var versionProviderHelper = new VersionProviderHelper(message, createTime, m_bookRepository, m_versionIdGenerator);

            var settings = new DocxToTeiConverterSettings
            {
                Debug = false,
                InputFilePath = inputFileResource.FullPath,
                MetadataFilePath = m_conversionMetadataPath,
                OutputFilePath = bookResource.FullPath,
                TempDirectoryPath = tmpDirPath,
                GetVersionList = versionProviderHelper.GetVersionsByBookId,
                SplitDocumentByPageBreaks = true
            };

            var converter = new DocxToTeiConverter();
            var conversionResult = converter.Convert(settings);

            if (conversionResult.IsConverted)
            {
                resourceSessionDirector.Resources.Add(metaDataResource);
                resourceSessionDirector.Resources.Add(bookResource);
            }
            else
            {
                throw new ConversionException(string.Format("Soubor se nepodařilo konvertovat. Viz vnitřní výjimka : '{0}'", conversionResult.Errors.FirstOrDefault()));
            }
        }
    }

    public class ConversionException : Exception
    {
        public ConversionException(string message) : base(message)
        {
        }

        public ConversionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class VersionProviderHelper
    {
        private readonly BookRepository m_bookRepository;
        private readonly DateTime m_createTime;
        private readonly string m_message;
        private readonly VersionIdGenerator m_versionIdGenerator;

        public VersionProviderHelper(string message, DateTime createTime, BookRepository bookRepository, VersionIdGenerator versionIdGenerator)
        {
            m_message = message;
            m_createTime = createTime;
            m_bookRepository = bookRepository;
            m_versionIdGenerator = versionIdGenerator;
        }

        public List<VersionInfoSkeleton> GetVersionsByBookId(string bookId)
        {
            var versions = m_bookRepository.GetAllVersionsByBookId(bookId);
            var vers = versions.Select(x => new VersionInfoSkeleton(x.Description, x.CreateTime)).ToList();
            vers.Add(new VersionInfoSkeleton(m_message, m_createTime, m_versionIdGenerator.Generate(m_createTime)));
            return vers;
        }
    }
}