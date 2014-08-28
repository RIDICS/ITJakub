using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Hangman.DataContract
{
    public class HangmanTaskContract
    {
        [JsonProperty("Word")]
        public string Word { get; set; }
    }
}
