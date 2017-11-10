using System.Runtime.Serialization;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Shared.DataContracts.Search.Criteria
{
    [DataContract]
    [KnownType(typeof(DatingListCriteriaContract))]
    [KnownType(typeof(WordListCriteriaContract))]
    [KnownType(typeof(ResultCriteriaContract))]
    [KnownType(typeof(ResultRestrictionCriteriaContract))]
    [KnownType(typeof(SnapshotResultRestrictionCriteriaContract))]
    [KnownType(typeof(TokenDistanceListCriteriaContract))]
    [KnownType(typeof(RegexSearchCriteriaContract))]
    [KnownType(typeof(RegexTokenListCriteriaContract))]
    [KnownType(typeof(SelectedCategoryCriteriaContract))]
    [KnownType(typeof(AuthorizationCriteriaContract))]
    public abstract class SearchCriteriaContract
    {
        [DataMember]
        public virtual CriteriaKey Key { get; set; }
    }
}