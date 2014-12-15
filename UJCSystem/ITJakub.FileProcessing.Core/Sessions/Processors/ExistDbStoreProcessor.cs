using System.Collections.Generic;
using System.IO;
using System.Linq;
using ITJakub.Core.Resources;
using ITJakub.Core.SearchService;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class ExistDbStoreProcessor
    {
        private readonly SearchServiceClient m_searchServiceClient;

        public ExistDbStoreProcessor(SearchServiceClient searchServiceClient)
        {
            m_searchServiceClient = searchServiceClient;
        }

        public void Process(string bookId, string versionId, IEnumerable<Resource> resources)
        {
            List<Resource> existResources =
                resources.Where(
                    resource =>
                        resource.ResourceType == ResourceTypeEnum.Transformation ||
                        resource.ResourceType == ResourceTypeEnum.Book || resource.ResourceType == ResourceTypeEnum.Page)
                    .ToList();
            foreach (Resource resource in existResources)
            {
                m_searchServiceClient.UploadFile(new FileUploadContract
                {
                    BookId = bookId,
                    BookVersionid = versionId,
                    FileName = resource.FileName,
                    ResourceType = resource.ResourceType,
                    DataStream = File.Open(resource.FullPath, FileMode.Open)
                });
            }
        }
    }
}