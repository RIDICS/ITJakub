﻿using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    [KnownType(typeof(DatingCriteriaContract))]
    [KnownType(typeof(WordListCriteriaContract))]
    [KnownType(typeof(ResultCriteriaContract))]
    [KnownType(typeof(ResultRestrictionCriteriaContract))]
    [KnownType(typeof(TokenDistanceListCriteriaContract))]
    [KnownType(typeof(RegexSearchCriteriaContract))]
    [KnownType(typeof(RegexTokenListCriteriaContract))]
    public abstract class SearchCriteriaContract
    {
        [DataMember]
        public virtual CriteriaKey Key { get; set; }
    }
}