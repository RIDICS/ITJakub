using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    [KnownType(typeof(StringCriteriaContract))]
    public abstract class SearchCriteriaContract
    {
        [DataMember]
        public CriteriaKey Key { get; set; }
    }

    [DataContract]
    public class StringCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public string Value { get; set; }
    }

    public enum CriteriaKey
    {
        Author,
        Title
    }
}