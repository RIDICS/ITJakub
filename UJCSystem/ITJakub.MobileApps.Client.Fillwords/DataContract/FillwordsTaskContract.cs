using System.Collections.Generic;

namespace ITJakub.MobileApps.Client.Fillwords.DataContract
{
    public class FillwordsTaskContract
    {
        public string DocumentRtf { get; set; }

        public IList<OptionsTaskContract> Options { get; set; } 

        public class OptionsTaskContract
        {
            public int WordPosition { get; set; }

            public IList<string> Options { get; set; } 
        }
    }
}
