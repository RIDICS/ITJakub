using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Hangman.DataContract
{
    public class GuessLetterContract
    {
        [JsonProperty("Letter")]
        public char Letter { get; set; }

        [JsonProperty("WordOrder")]
        public int WordOrder { get; set; }
    }
}
