namespace ITJakub.Web.Hub.Models.Requests.Lemmatization
{
    public class AddTokenCharacteristicRequest
    {
        public long TokenId { get; set; }

        public string MorphologicalCharacteristic { get; set; }

        public string Description { get; set; }
    }
}