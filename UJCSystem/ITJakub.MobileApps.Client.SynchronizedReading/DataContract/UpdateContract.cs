using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.SynchronizedReading.DataContract
{
    public class UpdateContract
    {
        [JsonProperty("SelectionStart")]
        public int SelectionStart { get; set; }

        [JsonProperty("SelectionLength")]
        public int SelectionLength { get; set; }
    }
}
