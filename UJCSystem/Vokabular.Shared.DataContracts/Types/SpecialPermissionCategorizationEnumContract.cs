using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.Shared.DataContracts.Types
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpecialPermissionCategorizationEnumContract : byte
    {
        [EnumMember] Action = 0,
        [EnumMember] Autoimport = 1,
        [EnumMember] Read = 2
    }
}