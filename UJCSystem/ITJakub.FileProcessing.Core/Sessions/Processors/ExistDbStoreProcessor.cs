using System.Collections.Generic;
using System.IO;
using System.Linq;
using ITJakub.Core.Resources;
using ITJakub.Core.SearchService;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class ExistDbStoreProcessor : IResourceProcessor
    {
        private readonly SearchServiceClient m_searchServiceClient;

        public ExistDbStoreProcessor(SearchServiceClient searchServiceClient)
        {
            m_searchServiceClient = searchServiceClient;
        }

        public void Process(ResourceSessionDirector resourceDirector)
        {
            List<Resource> existResources =
                resourceDirector.Resources.Where(
                    resource =>
                        resource.ResourceType == ResourceTypeEnum.Transformation ||
                        resource.ResourceType == ResourceTypeEnum.Book || resource.ResourceType == ResourceTypeEnum.Page)
                    .ToList();
            foreach (Resource resource in existResources)
            {
                m_searchServiceClient.UploadFile(new FileUploadContract
                {
                    BookId = resourceDirector.GetSessionInfoValue<string>(SessionInfo.BookId),
                    BookVersionid = resourceDirector.GetSessionInfoValue<string>(SessionInfo.VersionId),
                    FileName = resource.FileName,
                    ResourceType = resource.ResourceType,
                    DataStream = File.Open(resource.FullPath, FileMode.Open)
                });
            }
        }
    }
}