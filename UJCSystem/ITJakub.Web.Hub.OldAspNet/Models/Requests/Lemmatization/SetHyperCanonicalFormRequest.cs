namespace ITJakub.Web.Hub.Models.Requests.Lemmatization
{
    public class SetHyperCanonicalFormRequest
    {
        public long CanonicalFormId { get; set; }

        public long HyperCanonicalFormId { get; set; }
    }
}