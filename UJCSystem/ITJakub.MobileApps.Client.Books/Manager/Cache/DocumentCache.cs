using System.IO;
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

        protected override async Task<string> GetFromServerAsync(string bookGuid, string pageId)
        {
            try
            {
                using (var pageStream = await m_serviceClient.GetPageAsRtfAsync(bookGuid, pageId))
                using (var streamReader = new StreamReader(pageStream))
                {
                    var text = streamReader.ReadToEnd();
                    return text;
                }
            }
            catch (IOException exception)
            {
                throw new MobileCommunicationException(exception);
            }
        }
    }
}