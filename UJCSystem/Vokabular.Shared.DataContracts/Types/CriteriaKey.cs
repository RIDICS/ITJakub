using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.Shared.DataContracts.Types
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
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
        [EnumMember] Headword = 10,
        [EnumMember] HeadwordDescription = 11,
        [EnumMember] HeadwordDescriptionTokenDistance = 12,
        [EnumMember] SelectedCategory = 13,
        [EnumMember] Term = 14,
        [EnumMember] Authorization = 15,
    }
}