using System.Collections.Generic;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Fillwords.DataContract
{
    public class FillwordsEvaluationContract
    {
        [JsonProperty("CorrectAnswers")]
        public int CorrectAnswers { get; set; }

        [JsonProperty("AnswerList")]
        public IList<string> AnswerList { get; set; }
    }
}
