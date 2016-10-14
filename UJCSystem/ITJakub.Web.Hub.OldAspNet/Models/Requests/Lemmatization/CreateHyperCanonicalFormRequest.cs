using ITJakub.Lemmatization.Shared.Contracts;

namespace ITJakub.Web.Hub.Models.Requests.Lemmatization
{
    public class CreateHyperCanonicalFormRequest
    {
        public long CanonicalFormId { get; set; }

        public HyperCanonicalFormTypeContract Type { get; set; }

        public string Text { get; set; }

        public string Description { get; set; }
    }
}