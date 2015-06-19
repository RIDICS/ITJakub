using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    [KnownType(typeof(StringCriteriaContract))]
    [KnownType(typeof(StringListCriteriaContract))]
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

    [DataContract]
    public class StringListCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public List<string> Values { get; set; }
    }

    public enum CriteriaKey
    {
        Author,
        Title,
        Editor
    }
}