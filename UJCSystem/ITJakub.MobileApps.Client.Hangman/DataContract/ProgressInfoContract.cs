using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Hangman.DataContract
{
    public class ProgressInfoContract
    {
        [JsonProperty("HangmanCount")]
        public int HangmanCount { get; set; }

        [JsonProperty("LivesRemain")]
        public int LivesRemain { get; set; }

        [JsonProperty("WordOrder")]
        public int WordOrder { get; set; }

        [JsonProperty("LetterCount")]
        public int LetterCount { get; set; }

        [JsonProperty("Win")]
        public bool Win { get; set; }

        [JsonProperty("HangmanPicture")]
        public int HangmanPicture { get; set; }

        [JsonProperty("Letter")]
        public char Letter { get; set; }
    }
}