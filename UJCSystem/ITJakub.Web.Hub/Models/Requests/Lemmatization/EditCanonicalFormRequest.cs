using ITJakub.Lemmatization.Shared.Contracts;

namespace ITJakub.Web.Hub.Models.Requests.Lemmatization
{
    public class EditCanonicalFormRequest
    {
        public long CanonicalFormId { get; set; }

        public string Text { get; set; }

        public CanonicalFormTypeContract Type { get; set; }

        public string Description { get; set; }
    }
}