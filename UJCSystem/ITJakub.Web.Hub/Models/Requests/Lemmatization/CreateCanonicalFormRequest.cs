using ITJakub.Lemmatization.Shared.Contracts;

namespace ITJakub.Web.Hub.Models.Requests.Lemmatization
{
    public class CreateCanonicalFormRequest
    {
        public long TokenCharacteristicId { get; set; }

        public CanonicalFormTypeContract Type { get; set; }

        public string Text { get; set; }

        public string Description { get; set; }
    }
}