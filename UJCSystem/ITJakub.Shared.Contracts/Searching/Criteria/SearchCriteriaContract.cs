using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    [KnownType(typeof(DatingListCriteriaContract))]
    [KnownType(typeof(WordListCriteriaContract))]
    [KnownType(typeof(ResultCriteriaContract))]
    [KnownType(typeof(ResultRestrictionCriteriaContract))]
    [KnownType(typeof(TokenDistanceListCriteriaContract))]
    [KnownType(typeof(RegexSearchCriteriaContract))]
    [KnownType(typeof(RegexTokenListCriteriaContract))]
    [KnownType(typeof(SelectedCategoryCriteriaContract))]
    public abstract class SearchCriteriaContract
    {
        [DataMember]
        public virtual CriteriaKey Key { get; set; }
    }
}