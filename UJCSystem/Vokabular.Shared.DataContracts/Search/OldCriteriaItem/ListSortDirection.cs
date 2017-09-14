using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.OldCriteriaItem
{
    [DataContract]
    public enum ListSortDirection
    {
        [EnumMember]
        Ascending = 0,
        [EnumMember]
        Descending = 1,
    }
}