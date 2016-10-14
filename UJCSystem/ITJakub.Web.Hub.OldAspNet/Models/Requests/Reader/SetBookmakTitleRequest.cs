namespace ITJakub.Web.Hub.Models.Requests.Reader
{
    public class SetBookmakTitleRequest
    {
        public string BookId { get; set; }

        public string PageXmlId { get; set; }

        public string Title { get; set; }
    }
}