using System.Runtime.Serialization;

namespace Vokabular.ForumSite.Core.Helpers
{
    public enum UrlBookTypeEnum : short
    {
        Dictionaries = 1,
        Editions = 0,
        BohemianTextBank = 4,
        OldGrammar = 2,
        ProfessionalLiterature = 3,
        Bibliographies = 5,
        CardFiles = 6,
        AudioBooks = 7,
    }
}