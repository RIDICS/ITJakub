using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Notes
{
    [DataContract]
    public class FeedbackCriteriaContract
    {

        [DataMember]
        public IList<FeedbackCategoryEnumContract> Categories { get; set; }

        [DataMember]
        public int? Start { get; set; }

        [DataMember]
        public int? Count { get; set; }

    }
}