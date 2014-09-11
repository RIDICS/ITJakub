using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Hangman.DataContract
{
    public class ProgressInfoContract
    {
        [JsonProperty("Lives")]
        public int Lives { get; set; }

        [JsonProperty("LetterCount")]
        public int LetterCount { get; set; }

        [JsonProperty("Win")]
        public bool Win { get; set; }
    }
}