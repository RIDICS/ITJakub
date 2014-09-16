using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Crosswords.DataContract
{
    public class ProgressInfoContract
    {
        [JsonProperty("RowIndex")]
        public int RowIndex { get; set; }

        [JsonProperty("FilledCharacters")]
        public int FilledCharacters { get; set; }

        [JsonProperty("IsCorrect")]
        public bool IsCorrect { get; set; }
    }
}
