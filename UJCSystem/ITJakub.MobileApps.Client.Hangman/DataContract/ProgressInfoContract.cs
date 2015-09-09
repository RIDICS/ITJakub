using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Hangman.DataContract
{
    public class ProgressInfoContract
    {
        [JsonProperty("HangmanCount")]
        public int HangmanCount { get; set; }

        [JsonProperty("LivesRemain")]
        public int LivesRemain { get; set; }

        [JsonProperty("WordCount")]
        public int GuessedWordCount { get; set; }

        [JsonProperty("LetterCount")]
        public int LetterCount { get; set; }

        [JsonProperty("Win")]
        public bool Win { get; set; }
    }
}