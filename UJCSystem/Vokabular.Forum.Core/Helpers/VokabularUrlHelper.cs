using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Vokabular.ForumSite.Core.Helpers
{
    public static class VokabularUrlHelper
    {
        public const string VokabularBaseUrl = "https://localhost:44368"; //TODO load from config file???

        public static string GetBookUrl(long bookId, short bookTypeId)
        {
            return VokabularBaseUrl + "/" + (UrlBookTypeEnum)bookTypeId + "/" + (UrlBookTypeEnum)bookTypeId + "/listing?bookId=" + bookId; 
        }
    }
}