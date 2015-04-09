using System.Threading.Tasks;
using ITJakub.MobileApps.MobileContracts;

namespace ITJakub.MobileApps.Client.Books.Manager.Cache
{
    public class DocumentCache : CacheBase<string>
    {
        private readonly IMobileAppsService m_serviceClient;

        public DocumentCache(IMobileAppsService serviceClient, int maxSize) : base(maxSize)
        {
            m_serviceClient = serviceClient;
        }

        protected override Task<string> GetFromServerAsync(string bookGuid, string pageId)
        {
            return m_serviceClient.GetPageAsRtfAsync(bookGuid, pageId);
        }
    }
}