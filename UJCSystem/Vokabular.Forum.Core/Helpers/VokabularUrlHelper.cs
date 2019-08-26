namespace Vokabular.ForumSite.Core.Helpers
{
    public class VokabularUrlHelper
    {
        public string GetBookUrl(long bookId, string hostUrl)
        {
            return $"{hostUrl}BookReader/BookReader/Listing?bookId={bookId}";
        }
    }
}