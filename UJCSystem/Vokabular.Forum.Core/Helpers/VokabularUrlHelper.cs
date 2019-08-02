namespace Vokabular.ForumSite.Core.Helpers
{
    public class VokabularUrlHelper
    {
        public string GetBookUrl(long bookId, short bookTypeId, string hostUrl)
        {
            UrlBookTypeEnum urlBookType = (UrlBookTypeEnum) bookTypeId == UrlBookTypeEnum.BohemianTextBank
                ? UrlBookTypeEnum.Editions
                : (UrlBookTypeEnum) bookTypeId;
            return hostUrl + "/" + urlBookType + "/" + urlBookType + "/listing?bookId=" + bookId; 
        }
    }
}