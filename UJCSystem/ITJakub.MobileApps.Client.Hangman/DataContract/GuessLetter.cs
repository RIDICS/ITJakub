using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Hangman.DataContract
{
    public class GuessLetter
    {
        [JsonProperty("Letter")]
        public char Letter { get; set; }

        [JsonProperty("WordOrder")]
        public int WordOrder { get; set; }
    }
}
