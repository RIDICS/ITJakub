using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Books.Service.Client;

namespace ITJakub.MobileApps.Client.Books.Manager.Cache
{
    public class PhotoCache : CacheBase<BitmapImage>
    {
        private readonly IServiceClient m_serviceClient;

        public PhotoCache(IServiceClient serviceClient, int maxSize) : base(maxSize)
        {
            m_serviceClient = serviceClient;
        }

        protected override async Task<BitmapImage> GetFromServerAsync(string bookGuid, string pageId)
        {
            try
            {
                using (var stream = await m_serviceClient.GetPagePhotoAsync(bookGuid, pageId))
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    if (memoryStream.Length == 0)
                        throw new NotFoundException(null);

                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(memoryStream.AsRandomAccessStream());

                    return bitmapImage;
                }
            }
            catch (IOException exception)
            {
                throw new MobileCommunicationException(exception);
            }
        }
    }
}
