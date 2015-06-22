using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    [KnownType(typeof(DatingCriteriaContract))]
    [KnownType(typeof(WordListCriteriaContract))]
    public abstract class SearchCriteriaContract
    {
        [DataMember]
        public CriteriaKey Key { get; set; }
    }

    [DataContract]
    public class DatingCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public DateTime NotBefore { get; set; }

        [DataMember]
        public DateTime NotAfter { get; set; }
    }

    [DataContract]
    public class WordListCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public List<WordCriteriaContract> Values { get; set; }
    }

    [DataContract]
    public class WordCriteriaContract
    {
        [DataMember]
        public string StartsWith { get; set; }

        [DataMember]
        public List<string> Contains { get; set; }

        [DataMember]
        public string EndsWith { get; set; }
    }

    public enum CriteriaKey
    {
        Author,
        Title,
        Editor,
        Dating
    }
}