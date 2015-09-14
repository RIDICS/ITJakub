using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Books.Service.Client;

namespace ITJakub.MobileApps.Client.Books.Manager.Cache
{
    public class DocumentCache : CacheBase<string>
    {
        private readonly IBookServiceClient m_serviceClient;

        public DocumentCache(IBookServiceClient serviceClient, int maxSize) : base(maxSize)
        {
            m_serviceClient = serviceClient;
        }

        protected override Task<string> GetFromServerAsync(string bookGuid, string pageId)
        {
            return m_serviceClient.GetPageAsRtfAsync(bookGuid, pageId);
        }
    }
}