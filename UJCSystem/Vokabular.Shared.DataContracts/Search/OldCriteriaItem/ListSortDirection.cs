using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.OldCriteriaItem
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/System.ComponentModel", Name = "ListSortDirection")]
    public enum ListSortDirection
    {
        [EnumMember]
        Ascending = 0,
        [EnumMember]
        Descending = 1,
    }
}