using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.Sessions.Processors.Fulltext;
using ITJakub.Shared.Contracts;
using ITJakub.SearchService.DataContracts;
using ITJakub.SearchService.DataContracts.Contracts;
using log4net;
using Vokabular.Core.Storage.Resources;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class FulltextDbStoreProcessor : IResourceProcessor
    {
        private readonly IFulltextResourceProcessor m_fulltextResourceProcessor;

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public FulltextDbStoreProcessor(IFulltextResourceProcessor fulltextResourceProcessor)
        {
            m_fulltextResourceProcessor = fulltextResourceProcessor;
        }

        public void Process(ResourceSessionDirector resourceDirector)
        {
            var existFileResources =
                resourceDirector.Resources.Where(
                    resource =>
                        resource.ResourceType == ResourceType.Book ||
                        resource.ResourceType == ResourceType.Page ||
                        resource.ResourceType == ResourceType.BibliographyDocument ||
                        resource.ResourceType == ResourceType.Transformation);

            var bookData = resourceDirector.GetSessionInfoValue<BookData>(SessionInfo.BookData);

            foreach (var resource in existFileResources)
            {
                if (string.IsNullOrEmpty(resource.FileName))
                {
                    if (m_log.IsErrorEnabled)
                        m_log.ErrorFormat("Resource of type {0} and path {1} does not have fileName", resource.ResourceType, resource.FullPath);

                    continue;
                }

                using (var dataStream = File.Open(resource.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var resourceUploadContract = GetVersionResourceUploadContract(resource, resourceDirector, dataStream);

                    switch (resource.ResourceType)
                    {
                        case ResourceType.BibliographyDocument:
                            m_fulltextResourceProcessor.UploadBibliographyFile(resourceUploadContract);
                            break;

                        case ResourceType.Book:
                            m_fulltextResourceProcessor.UploadFullbookToBookVersion(resourceUploadContract);
                            break;

                        case ResourceType.Page:
                            var pageId = m_fulltextResourceProcessor.UploadPageToBookVersion(resourceUploadContract);
                            if (pageId != null)
                            {
                                var bookPageData = bookData.Pages.FirstOrDefault(x => x.XmlResource == resource.FileName);
                                if (bookPageData != null)
                                {
                                    bookPageData.XmlId = pageId;
                                }
                            }
                            break;

                        case ResourceType.Transformation:
                            m_fulltextResourceProcessor.UploadTransformationResource(resourceUploadContract);
                            break;
                            
                        default:
                            throw new ArgumentException($"ResourceType: '{resource.ResourceType}' not meant for ExistDb upload");
                    }
                }
            }
        }

        private VersionResourceUploadContract GetVersionResourceUploadContract(FileResource resource, ResourceSessionDirector resourceDirector, Stream dataStream)
        {
            return new VersionResourceUploadContract
            {
                BookId = resourceDirector.GetSessionInfoValue<string>(SessionInfo.BookXmlId),
                BookVersionId = resourceDirector.GetSessionInfoValue<string>(SessionInfo.VersionXmlId),
                FileName = resource.FileName,
                ResourceType = resource.ResourceType,
                DataStream = dataStream
            };
        }
    }
}