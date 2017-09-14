using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.OldCriteriaItem
{
    [DataContract]
    public class TermsSettingsContract
    {
        // count and start ommited because missing use case. This contract is used as request for loading terms.
    }
}