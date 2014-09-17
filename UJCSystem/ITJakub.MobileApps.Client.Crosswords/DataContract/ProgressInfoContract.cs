using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Crosswords.DataContract
{
    public class ProgressInfoContract
    {
        [JsonProperty("RowIndex")]
        public int RowIndex { get; set; }

        [JsonProperty("FilledWord")]
        public string FilledWord { get; set; }

        [JsonProperty("IsCorrect")]
        public bool IsCorrect { get; set; }
    }
}
