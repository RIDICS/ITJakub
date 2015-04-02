using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.SynchronizedReading.DataContract
{
    public class SelectedBookTaskContract
    {
        [JsonProperty("BookGuid")]
        public string BookGuid { get; set; }

        [JsonProperty("DefaultPageId")]
        public string DefaultPageId { get; set; }
    }
}
