namespace ITJakub.Web.Hub.Models.Requests.Lemmatization
{
    public class AddCanonicalFormRequest
    {
        public long TokenCharacteristicId { get; set; }

        public long CanonicalFormId { get; set; }
    }
}