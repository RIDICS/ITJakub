using System;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Notes
{
    [DataContract]
    public class FeedbackContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public UserContract User { get; set; }

        [DataMember]
        public string FilledName { get; set; }

        [DataMember]
        public string FilledEmail { get; set; }

        [DataMember]
        public FeedbackCategoryEnumContract Category { get; set; }

    }
}
