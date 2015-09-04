using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    public class TermsSettingsContract
    {
        // count and start ommited because missing use case. This contract is used as request for loading terms.
    }
}