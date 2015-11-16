using System.Collections.Generic;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Fillwords2.DataContract
{
    public class FillwordsAnswerContract
    {
        [JsonProperty("WordPosition")]
        public int WordPosition { get; set; }

        [JsonProperty("Answers")]
        public IList<OptionContract> Answers { get; set; }


        public class OptionContract
        {
            [JsonProperty("StartPosition")]
            public int StartPosition { get; set; }

            [JsonProperty("Answer")]
            public string Answer { get; set; }
        }
    }
}