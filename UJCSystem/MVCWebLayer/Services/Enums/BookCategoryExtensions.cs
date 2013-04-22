namespace Ujc.Naki.MVCWebLayer.Services.Enums
{
    public static class BookCategoryExtensions
    {
        public static string CzechString(this BookCategory e)
        {
            switch (e)
            {
                case BookCategory.Dictionary:
                    return "Slovníky";
                case BookCategory.Digitalized:
                    return "Digitalizované mluvnice";
                case BookCategory.Historic:
                    return "Historické texty";
                case BookCategory.Listed:
                    return "Lístkové kartotéky";
                case BookCategory.Technical:
                    return "Odborné texty";
                default:
                    return string.Empty;
            }
        }
    }
}