namespace ITJakub.Web.Hub.Models.Requests.Lemmatization
{
    public class EditTokenRequest
    {
        public long TokenId { get; set; }

        public string Description { get; set; }
    }
}