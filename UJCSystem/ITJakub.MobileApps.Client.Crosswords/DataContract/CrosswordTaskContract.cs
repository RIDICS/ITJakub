using System.Collections.Generic;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Crosswords.DataContract
{
    public class CrosswordTaskContract
    {
        [JsonProperty("AnswerPosition")]
        public int AnswerPosition { get; set; }

        [JsonProperty("RowList")]
        public List<RowContract> RowList { get; set; }


        public class RowContract
        {
            [JsonProperty("Label")]
            public string Label { get; set; }

            [JsonProperty("StartPosition")]
            public int StartPosition { get; set; }

            [JsonProperty("Answer")]
            public string Answer { get; set; }
        }
    }
}
