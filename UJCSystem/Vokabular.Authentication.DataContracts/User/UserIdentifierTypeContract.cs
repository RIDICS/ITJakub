using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.Authentication.DataContracts.User
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserIdentifierTypeContract
    {
        MasterUserId = 0,
        InsuranceNumber = 1,
        DoctorIdNumber = 2,
        DatabaseId = 3,
    }
}
