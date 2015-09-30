using System.Collections.Generic;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Fillwords2.DataContract
{
    public class FillwordsTaskContract
    {
        [JsonProperty("DocumentRtf")]
        public string DocumentRtf { get; set; }

        [JsonProperty("Options")]
        public IList<WordOptionsTaskContract> Options { get; set; } 


        public class WordOptionsTaskContract
        {
            [JsonProperty("WordPosition")]
            public int WordPosition { get; set; }

            [JsonProperty("WordList")]
            public IList<string> WordList { get; set; }

            [JsonProperty("CorrectAnswer")]
            public string CorrectAnswer { get; set; }
        }
    }
}
