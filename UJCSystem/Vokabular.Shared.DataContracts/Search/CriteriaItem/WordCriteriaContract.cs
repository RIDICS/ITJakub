using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.CriteriaItem
{
    [DataContract]
    public class WordCriteriaContract
    {
        [DataMember]
        public string StartsWith { get; set; }

        [DataMember]
        public IList<string> Contains { get; set; }

        [DataMember]
        public string EndsWith { get; set; }

        [DataMember]
        public string ExactMatch { get; set; }
    }
}