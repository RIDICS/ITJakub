using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    [KnownType(typeof (StringListCriteriaContract))]
    [KnownType(typeof (DatationCriteriaContract))]
    public abstract class SearchCriteriaContract
    {
        [DataMember]
        public CriteriaKey Key { get; set; }
    }

    [DataContract]
    public class StringListCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public List<string> Values { get; set; }
    }

    [DataContract]
    public class DatationCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public DateTime NotBefore { get; set; }

        [DataMember]
        public DateTime NotAfter { get; set; }
    }

    [DataContract]
    public enum CriteriaKey
    {
        [EnumMember] Author,
        [EnumMember] Title,
        [EnumMember] Editor,
        [EnumMember] Datation
    }
}