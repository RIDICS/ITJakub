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
        private readonly string m_dataDirectoryPath;
        private readonly VersionIdGenerator m_versionIdGenerator;

        public XmlConversionProcessor(string conversionMetadataPath, string dataDirectoryPath,
            BookRepository bookRepository, VersionIdGenerator versionIdGenerator)
        {
            m_conversionMetadataPath = conversionMetadataPath;
            m_bookRepository = bookRepository;
            m_versionIdGenerator = versionIdGenerator;
            m_dataDirectoryPath = dataDirectoryPath;
        }

        public void Process(ResourceSessionDirector resourceSessionDirector)
        {
            var inputFilesResource = resourceSessionDirector.Resources.Where(resource => resource.ResourceType == ResourceType.SourceDocument).OrderBy(r=>r.FileName, StringComparer.CurrentCultureIgnoreCase);
            var inputFileResource = inputFilesResource.First();

            string metaDataFileName;
            if (resourceSessionDirector.Resources.Any(x => x.ResourceType == ResourceType.UploadedMetadata))
            {
                metaDataFileName = string.Format("{0}_converted.xmd", Path.GetFileNameWithoutExtension(inputFileResource.FileName));
            }
            else
            {
                metaDataFileName = string.Format("{0}.xmd", Path.GetFileNameWithoutExtension(inputFileResource.FileName));
            }

            var metaDataResource = new Resource
            {
                FileName = metaDataFileName,
                FullPath = Path.Combine(resourceSessionDirector.SessionPath, metaDataFileName),
                ResourceType = ResourceType.ConvertedMetadata
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
                InputFilesPath = inputFilesResource.Select(p=>p.FullPath).ToArray(),
                UploadedFilesPath = resourceSessionDirector.Resources.GroupBy(resource => resource.ResourceType).
                    ToDictionary(resourceGroup => resourceGroup.Key,
                        resourceGroup => resourceGroup.Select(resource => resource.FullPath).ToArray()),
                MetadataFilePath = m_conversionMetadataPath,
                OutputDirectoryPath = resourceSessionDirector.SessionPath,
                OutputMetadataFilePath = metaDataResource.FullPath,
                TempDirectoryPath = tmpDirPath,
                GetVersionList = versionProviderHelper.GetVersionsByBookId,
                SplitDocumentByPageBreaks = true,
                DataDirectoryPath = m_dataDirectoryPath
            };

            var converter = new DocxToTeiConverter();
            var conversionResult = converter.Convert(settings);

            if (conversionResult.IsConverted)
            {
                resourceSessionDirector.Resources.Add(metaDataResource);                
            }
            else
            {
                var exception = conversionResult.Errors.FirstOrDefault();
                throw new ConversionException(string.Format("File was not converted sucessfully. See InnerException : '{0}'", exception), exception);
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

        public VersionProviderHelper(string message, DateTime createTime, BookRepository bookRepository,
            VersionIdGenerator versionIdGenerator)
        {
            m_message = message;
            m_createTime = createTime;
            m_bookRepository = bookRepository;
            m_versionIdGenerator = versionIdGenerator;
        }

        public List<VersionInfoSkeleton> GetVersionsByBookId(string bookId)
        {
            var versions = m_bookRepository.GetAllVersionsByBookXmlId(bookId);
            var vers = versions.Select(x => new VersionInfoSkeleton(x.Description, x.CreateTime)).ToList();
            vers.Add(new VersionInfoSkeleton(m_message, m_createTime, m_versionIdGenerator.Generate(m_createTime)));
            return vers;
        }
    }
}