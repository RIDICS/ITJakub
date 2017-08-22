using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.MainService.DataContracts.Contracts.Type
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BookTypeEnumContract
    {
        Edition = 0,
        Dictionary = 1,
        Grammar = 2,
        ProfessionalLiterature = 3,
        TextBank = 4,
        BibliographicalItem = 5,
        CardFile = 6,
        AudioBook = 7
    }
}