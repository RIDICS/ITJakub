using System.Net.Http;

namespace Vokabular.MainService.Core.Utils
{
    public static class HttpClientExtensions
    {
        public static int ReadAsInt(this HttpContent content)
        {
            var stringContent = content.ReadAsStringAsync().GetAwaiter().GetResult();
            return int.Parse(stringContent);
        }
    }
}
