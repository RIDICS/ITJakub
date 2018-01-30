using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.OldCriteriaItem
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Criteria", Name = "TermsSettingsContract")]
    public class TermsSettingsContract
    {
        // count and start ommited because missing use case. This contract is used as request for loading terms.
    }
}