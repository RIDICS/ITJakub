using System.Collections.Generic;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel.Enum;
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

            [JsonProperty("OptionList")]
            public IList<OptionTaskContract> OptionList { get; set; }

            [JsonProperty("CorrectAnswer")]
            public string CorrectAnswer { get; set; }
        }

        public class OptionTaskContract
        {
            [JsonProperty("StartPosition")]
            public int StartPosition { get; set; }

            [JsonProperty("EndPosition")]
            public int EndPosition { get; set; }
            
            [JsonProperty("AnswerType")]
            public AnswerType AnswerType { get; set; }

            [JsonProperty("Options")]
            public IList<string> Options { get; set; } 
        }
    }
}
