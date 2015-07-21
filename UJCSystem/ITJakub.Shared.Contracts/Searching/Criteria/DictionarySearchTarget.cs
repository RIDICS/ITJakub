using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    public enum DictionarySearchTarget
    {
        [EnumMember] Headword = 0,
        [EnumMember] Fulltext = 1,
    }
}