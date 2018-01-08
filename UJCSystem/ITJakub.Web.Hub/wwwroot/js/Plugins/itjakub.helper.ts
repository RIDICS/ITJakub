class BookTypeHelper {
    public static getText(bookType: BookTypeEnum): string {
        switch (bookType) {
            case BookTypeEnum.Edition:
                return "Edice";
            case BookTypeEnum.Dictionary:
                return "Slovníky";
            case BookTypeEnum.Grammar:
                return "Digitalizované mluvnice";
            case BookTypeEnum.ProfessionalLiterature:
                return "Odborná literatura";
            case BookTypeEnum.TextBank:
                return "Textová banka";
            case BookTypeEnum.BibliographicalItem:
                return "Bibliografie";
            case BookTypeEnum.CardFile:
                return "Kartotéky";
            case BookTypeEnum.AudioBook:
                return "Audio knihy";
            default:
                return "(neznámý typ knihy)";
        }
    }
}