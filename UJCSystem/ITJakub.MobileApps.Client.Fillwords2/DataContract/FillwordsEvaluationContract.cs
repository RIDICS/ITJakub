using System.Collections.Generic;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel.Enum;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Fillwords2.DataContract
{
    public class FillwordsEvaluationContract
    {
        [JsonProperty("CorrectAnswers")]
        public int CorrectAnswers { get; set; }

        [JsonProperty("OptionsCount")]
        public int OptionsCount { get; set; }

        [JsonProperty("AnswerList")]
        public IList<AnswerContract> AnswerList { get; set; }


        public class AnswerContract
        {
            [JsonProperty("Answer")]
            public string Answer { get; set; }

            [JsonProperty("AnswerState")]
            public AnswerState AnswerState { get; set; }
        }
    }
}
