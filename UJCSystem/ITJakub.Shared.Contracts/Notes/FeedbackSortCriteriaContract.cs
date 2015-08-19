using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Notes
{
    [DataContract]
    public class FeedbackSortCriteriaContract
    {
        [DataMember]
        public FeedbackSortEnum SortByField { get; set; }

        [DataMember]
        public bool SortAsc { get; set; }
         
    }
}