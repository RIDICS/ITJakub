using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.MainService.DataContracts.Contracts.Type
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FeedbackCategoryEnumContract : short
    {
        None = 0,
        Dictionaries = 1,
        Editions = 2,
        BohemianTextBank = 3,
        OldGrammar = 4,
        ProfessionalLiterature = 5,
        Bibliographies = 6,
        CardFiles = 7,
        AudioBooks = 8,
        Tools = 9,
    }
}