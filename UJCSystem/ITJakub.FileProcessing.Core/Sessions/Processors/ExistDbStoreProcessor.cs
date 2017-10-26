using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ITJakub.SearchService.DataContracts;
using ITJakub.SearchService.DataContracts.Contracts;
using log4net;
using Vokabular.Core.Storage.Resources;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class ExistDbStoreProcessor : IResourceProcessor
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);        
        

        public void Process(ResourceSessionDirector resourceDirector)
        {
            var existFileResources =
                resourceDirector.Resources.Where(
                    resource =>
                        resource.ResourceType == ResourceType.Book ||
                        resource.ResourceType == ResourceType.Page ||
                        resource.ResourceType == ResourceType.BibliographyDocument ||
                        resource.ResourceType == ResourceType.Transformation);

            foreach (var resource in existFileResources)
            {
                if (string.IsNullOrEmpty(resource.FileName) && m_log.IsFatalEnabled)
                {
                    m_log.ErrorFormat("Resource of type {0} and path {1} does not have fileName", resource.ResourceType,
                        resource.FullPath);
                    continue;
                }

                using (var dataStream = File.Open(resource.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var resourceUploadContract = GetVersionResourceUploadContract(resource, resourceDirector, dataStream);

                    switch (resource.ResourceType)
                    {
                        case ResourceType.BibliographyDocument:
                            UploadBibliographyFile(resourceUploadContract);
                            break;

                        case ResourceType.Book:
                            UploadResourceToBookVersion(resourceUploadContract);
                            break;

                        case ResourceType.Page:
                            UploadResourceToBookVersion(resourceUploadContract);
                            break;

                        case ResourceType.Transformation:
                            UploadTransformationResource(resourceUploadContract);
                            break;

                        

                        default:
                            throw new ArgumentException($"ResourceType: '{resource.ResourceType}' not meant for ExistDb upload");
                    }
                }
            }
        }

        private void UploadTransformationResource(VersionResourceUploadContract resourceUploadContract)
        {
            using (var ssc = new SearchServiceClient())
            {
                ssc.UploadBookFile(resourceUploadContract);
            }
        }

        private void UploadBibliographyFile(VersionResourceUploadContract resourceUploadContract)
        {
            using (var ssc = new SearchServiceClient())
            {
                ssc.UploadBibliographyFile(resourceUploadContract);
            }
        }

        private void UploadResourceToBookVersion(VersionResourceUploadContract resourceUploadContract)
        {
            using (var ssc = new SearchServiceClient())
            {
                ssc.UploadVersionFile(resourceUploadContract);
            }
        }

        private VersionResourceUploadContract GetVersionResourceUploadContract(Resource resource, ResourceSessionDirector resourceDirector, Stream dataStream)
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