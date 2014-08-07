using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ITJakub.MobileApps.Client.Core.Manager
{
    public class UserAvatarCache
    {
        private const int AvatarImageSize = 40;
        private const uint BoundLocation = (uint)AvatarImageSize/2;
        private readonly Dictionary<long, UserAvatar> m_cache = new Dictionary<long, UserAvatar>();

        public void AddAvatarUrl(long userId, string userAvatarUrl)
        {
            if (!m_cache.ContainsKey(userId))
            {
                m_cache.Add(userId, new UserAvatar { SaveTime = DateTime.UtcNow, Url = userAvatarUrl });
            }
        }

        public async Task<ImageSource> GetUserAvatar(long userId)
        {
            if (m_cache.ContainsKey(userId))
            {
                UserAvatar user = m_cache[userId];
                if (user.ImageSource != null)
                    return user.ImageSource;

                BitmapImage userImage = await DownloadAndTranscodeImage(user.Url);
                user.ImageSource = userImage;

                return user.ImageSource;
            }

            return null;
        }

        private async Task<BitmapImage> DownloadAndTranscodeImage(string url)
        {
            var client = new HttpClient();
            try
            {
                Stream stream = await client.GetStreamAsync(url);
                var memStream = new MemoryStream();
                stream.CopyTo(memStream);
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(memStream.AsRandomAccessStream());


                var result = new InMemoryRandomAccessStream();
                BitmapEncoder enc = await BitmapEncoder.CreateForTranscodingAsync(result, decoder);
                enc.BitmapTransform.ScaledHeight = AvatarImageSize;
                enc.BitmapTransform.ScaledWidth = AvatarImageSize;

                var bounds = new BitmapBounds { Height = BoundLocation, Width = BoundLocation, X = BoundLocation, Y = BoundLocation };
                enc.BitmapTransform.Bounds = bounds;

                await enc.FlushAsync();

                var bImg = new BitmapImage();
                bImg.SetSource(result);
                return bImg;
            }
            catch (Exception ex)
            {//TODO determine exceptions
                return null;
            }
        }
    }

    public class UserAvatar
    {
        public DateTime SaveTime { get; set; }

        public ImageSource ImageSource { get; set; }

        public string Url { get; set; }
    }
}