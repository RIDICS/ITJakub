using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching
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

    [DataContract]
    public class DatingCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public DateTime? NotBefore { get; set; }

        [DataMember]
        public DateTime? NotAfter { get; set; }
    }

    [DataContract]
    public class WordListCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public IList<WordCriteriaContract> Disjunctions { get; set; }
    }

    [DataContract]
    public class WordCriteriaContract
    {
        [DataMember]
        public string StartsWith { get; set; }

        [DataMember]
        public IList<string> Contains { get; set; }

        [DataMember]
        public string EndsWith { get; set; }
    }

    [DataContract]
    public class ResultCriteriaContract : SearchCriteriaContract
    {
        public override CriteriaKey Key
        {
            get { return CriteriaKey.Result; }
        }

        [DataMember]
        public string ExistDbSession { get; set; }

        [DataMember]
        public int? Start { get; set; }

        [DataMember]
        public int? End { get; set; }

        [DataMember]
        public SortEnum? Sorting { get; set; }

        [DataMember]
        public ListSortDirection Direction { get; set; }
    }
    
    [DataContract]
    public class ResultRestrictionCriteriaContract : SearchCriteriaContract
    {
        public override CriteriaKey Key
        {
            get { return CriteriaKey.ResultRestriction; }
        }

        [DataMember]
        public IList<BookVersionPairContract> ResultBooks { get; set; }
    }

    [DataContract]
    public class TokenDistanceCriteriaContract
    {
        [DataMember]
        public int Distance { get; set; }

        [DataMember]
        public WordCriteriaContract First { get; set; }

        [DataMember]
        public WordCriteriaContract Second { get; set; }
    }

    [DataContract]
    public class TokenDistanceListCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public IList<TokenDistanceCriteriaContract> Disjunctions { get; set; } 
    }

	[DataContract]
    public enum CriteriaKey
    {
        [EnumMember] Author = 0,
        [EnumMember] Title = 1,
        [EnumMember] Editor = 2,
        [EnumMember] Dating = 3,
        [EnumMember] Fulltext = 4,
        [EnumMember] Heading = 5,
        [EnumMember] Sentence = 6,
        [EnumMember] Result = 7,
        [EnumMember] ResultRestriction = 8,
        [EnumMember] TokenDistance = 9,
    }
    
    [DataContract]
    public enum SortEnum
    {
        [EnumMember]
        Author = 0,
        [EnumMember]
        Title = 1,
        [EnumMember]
        Editor = 2,
        [EnumMember]
        Dating = 3,
    }
}