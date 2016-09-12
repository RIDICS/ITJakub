namespace ITJakub.Web.Hub.Models.Requests.Lemmatization
{
    public class EditTokenCharacteristicRequest
    {
        public long TokenCharacteristicId { get; set; }

        public string MorphologicalCharacteristic { get; set; }

        public string Description { get; set; }
    }
}