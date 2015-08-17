using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.Shared.Contracts.Notes
{
    [DataContract]
    public class FeedbackContract
    {

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public string FilledName { get; set; }

        [DataMember]
        public string FilledEmail { get; set; }

        [DataMember]
        public FeedbackCategoryEnumContract Category { get; set; }

    }
}
