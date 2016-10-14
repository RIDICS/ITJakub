using ITJakub.Lemmatization.Shared.Contracts;

namespace ITJakub.Web.Hub.Models.Requests.Lemmatization
{
    public class EditHyperCanonicalFormRequest
    {
        public long HyperCanonicalFormId { get; set; }

        public string Text { get; set; }

        public HyperCanonicalFormTypeContract Type { get; set; }

        public string Description { get; set; }
    }
}