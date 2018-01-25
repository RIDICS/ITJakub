using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Helpers
{
    public class BookTypeHelper
    {
        public static string GetCategoryName(BookTypeEnumContract bookType)
        {
            switch (bookType)
            {
                case BookTypeEnumContract.Edition:
                    return "Edice";
                case BookTypeEnumContract.Dictionary:
                    return "Slovníky";
                case BookTypeEnumContract.Grammar:
                    return "Digitalizované mluvnice";
                case BookTypeEnumContract.ProfessionalLiterature:
                    return "Odborná literatura";
                case BookTypeEnumContract.TextBank:
                    return "Textová banka";
                case BookTypeEnumContract.BibliographicalItem:
                    return "Bibliografie";
                case BookTypeEnumContract.CardFile:
                    return "Kartotéky";
                case BookTypeEnumContract.AudioBook:
                    return "Audio knihy";
                default:
                    return null;
            }
        }
    }
}
