using System.IO;
using System.Linq;
using System.Reflection;
using ITJakub.Core.SearchService;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Resources;
using log4net;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class ExistDbStoreProcessor : IResourceProcessor
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly SearchServiceClient m_searchServiceClient;

        public ExistDbStoreProcessor(SearchServiceClient searchServiceClient)
        {
            m_searchServiceClient = searchServiceClient;
        }

        public void Process(ResourceSessionDirector resourceDirector)
        {
            var existFileResources =
                resourceDirector.Resources.Where(
                    resource =>
                        resource.ResourceType == ResourceType.Book || resource.ResourceType == ResourceType.Page || resource.ResourceType == ResourceType.Transformation);
            foreach (var resource in existFileResources)
            {
                if (string.IsNullOrEmpty(resource.FileName) && m_log.IsFatalEnabled)
                {
                    m_log.ErrorFormat("Resource of type {0} and path {1} does not have fileName", resource.ResourceType,
                        resource.FullPath);
                    continue; //TODO maybe throw exception?
                }

                using (var dataStream = File.Open(resource.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    m_searchServiceClient.UploadVersionFileAsync(new VersionResourceUploadContract
                    {
                        BookId = resourceDirector.GetSessionInfoValue<string>(SessionInfo.BookId),
                        BookVersionId = resourceDirector.GetSessionInfoValue<string>(SessionInfo.VersionId),
                        FileName = resource.FileName,
                        ResourceType = resource.ResourceType,
                        DataStream = dataStream
                    });
                }
            }
        }
    }
}