using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.Shared.DataContracts.Types
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BookTypeEnumContract : byte
    {
        [EnumMember] Edition = 0, //Edice
        [EnumMember] Dictionary = 1, //Slovnik
        [EnumMember] Grammar = 2, //Mluvnice
        [EnumMember] ProfessionalLiterature = 3, //Odborna literatura
        [EnumMember] TextBank = 4, //Textova banka
        [EnumMember] BibliographicalItem = 5,   //Bibliograficky zaznam
        [EnumMember] CardFile = 6,   //Kartoteka
        [EnumMember] AudioBook = 7  //Audiokniha

    }
}