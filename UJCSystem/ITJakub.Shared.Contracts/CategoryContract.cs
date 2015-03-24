using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public class CategoryContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int? ParentCategoryId { get; set; }

    }
}