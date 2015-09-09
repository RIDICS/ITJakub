using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Hangman.DataContract
{
    public class HangmanTaskContract
    {
        [JsonProperty("Words")]
        public WordContract[] Words { get; set; }

        [JsonProperty("SpecialLetters")]
        public char[] SpecialLetters { get; set; }


        public class WordContract
        {
            [JsonProperty("Hint")]
            public string Hint { get; set; }

            [JsonProperty("Answer")]
            public string Answer { get; set; }
        }
    }
}
