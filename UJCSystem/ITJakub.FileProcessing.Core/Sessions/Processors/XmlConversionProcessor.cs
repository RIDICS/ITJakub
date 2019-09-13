using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ITJakub.FileProcessing.DataContracts;
using Ujc.Ovj.Ooxml.Conversion;
using Vokabular.Core.Storage.Resources;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class XmlConversionProcessor : IResourceProcessor
    {
        private readonly string m_conversionMetadataPath;
        private readonly string m_dataDirectoryPath;
        private readonly ProjectRepository m_projectRepository;
        private readonly VersionIdGenerator m_versionIdGenerator;

        public XmlConversionProcessor(string conversionMetadataPath, string dataDirectoryPath,
            ProjectRepository projectRepository, VersionIdGenerator versionIdGenerator)
        {
            m_conversionMetadataPath = conversionMetadataPath;
            m_dataDirectoryPath = dataDirectoryPath;
            m_projectRepository = projectRepository;
            m_versionIdGenerator = versionIdGenerator;
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

            var metaDataResource = new FileResource
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
            var projectType = (ProjectTypeEnum) resourceSessionDirector.GetSessionInfoValue<ProjectTypeContract>(SessionInfo.ProjectType);

            var versionProviderHelper = new VersionProviderHelper(message, createTime, m_projectRepository, m_versionIdGenerator, projectType);

            var settings = new DocxToTeiConverterSettings
            {
                Debug = false,
                InputFilesPath = inputFilesResource.Select(p=>p.FullPath).ToArray(),
                UploadedFilesPath = resourceSessionDirector.Resources.GroupBy(resource => resource.ResourceType).
                    ToDictionary(resourceGroup => resourceGroup.Key,
                        resourceGroup => resourceGroup.Select(resource => resource.FullPath).ToArray()),
                //MetadataFilePath = null,
                OutputDirectoryPath = resourceSessionDirector.SessionPath,
                OutputMetadataFilePath = metaDataResource.FullPath,
                TempDirectoryPath = tmpDirPath,
                GetVersionList = versionProviderHelper.GetVersionsByBookXmlId,
                SplitDocumentByPageBreaks = true,
                // HttpRuntime.BinDirectory is used instead of HostingEnvironment.ApplicationPhysicalPath because all required files are in bin folder
                DataDirectoryPath = Path.Combine(HttpRuntime.BinDirectory, m_dataDirectoryPath)
            };

            var evidenceFolderPath = Path.Combine(HttpRuntime.BinDirectory,
                m_conversionMetadataPath);
            var evidenceXmlFiles = Directory.GetFiles(evidenceFolderPath);
            ConversionResult conversionResult = null;

            foreach (var evidenceXmlFile in evidenceXmlFiles)
            {
                settings.MetadataFilePath = evidenceXmlFile;

                var converter = new DocxToTeiConverter();
                conversionResult = converter.Convert(settings);
                
                if (conversionResult.Errors.Count == 0)
                {
                    break;
                }
            }
            
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
        private readonly DateTime m_createTime;
        private readonly ProjectRepository m_projectRepository;
        private readonly string m_message;
        private readonly VersionIdGenerator m_versionIdGenerator;
        private readonly ProjectTypeEnum m_projectType;

        public VersionProviderHelper(string message, DateTime createTime, ProjectRepository projectRepository,
            VersionIdGenerator versionIdGenerator, ProjectTypeEnum projectType)
        {
            m_message = message;
            m_createTime = createTime;
            m_projectRepository = projectRepository;
            m_versionIdGenerator = versionIdGenerator;
            m_projectType = projectType;
        }

        public List<VersionInfoSkeleton> GetVersionsByBookXmlId(string bookXmlId)
        {
            var importLogs = m_projectRepository.InvokeUnitOfWork(x => x.GetAllImportLogByExternalId(bookXmlId, m_projectType));
            var vers = importLogs.Select(x => new VersionInfoSkeleton(x.AdditionalDescription, x.CreateTime)).ToList();
            vers.Add(new VersionInfoSkeleton(m_message, m_createTime, m_versionIdGenerator.Generate(m_createTime)));
            return vers;
        }
    }
}