using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Books.Service.Client;

namespace ITJakub.MobileApps.Client.Books.Manager.Cache
{
    public class DocumentCache : CacheBase<string>
    {
        private readonly IServiceClient m_serviceClient;

        public DocumentCache(IServiceClient serviceClient, int maxSize) : base(maxSize)
        {
            m_serviceClient = serviceClient;
        }

        protected override Task<string> GetFromServerAsync(string bookGuid, string pageId)
        {
            return m_serviceClient.GetPageAsRtfAsync(bookGuid, pageId);
        }
    }
}