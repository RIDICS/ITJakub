namespace Vokabular.ForumSite.Core.Helpers
{
    public static class VokabularUrlHelper
    {
        public const string VokabularBaseUrl = "https://localhost:44368"; //TODO load from config file???

        public static string GetBookUrl(long bookId, short bookTypeId)
        {
            UrlBookTypeEnum urlBookType = (UrlBookTypeEnum) bookTypeId == UrlBookTypeEnum.BohemianTextBank
                ? UrlBookTypeEnum.Editions
                : (UrlBookTypeEnum) bookTypeId;
            return VokabularBaseUrl + "/" + urlBookType + "/" + urlBookType + "/listing?bookId=" + bookId; 
        }
    }
}