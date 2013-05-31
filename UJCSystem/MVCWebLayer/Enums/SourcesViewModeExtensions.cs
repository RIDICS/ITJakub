namespace ITJakub.MVCWebLayer.Enums
{
    public static class SourcesViewModeExtensions
    {
        public static string ToUrlParam(this SourcesViewMode mode)
        {
            switch (mode)
            {
                case SourcesViewMode.Author:
                    return "autor";
                case SourcesViewMode.Name:
                    return "nazev";
                default:
                    return string.Empty;
            }
        }

        public static string ToCsName(this SourcesViewMode mode)
        {
            switch (mode)
            {
                case SourcesViewMode.Author:
                    return "Autor";
                case SourcesViewMode.Name:
                    return "Název";
                default:
                    return string.Empty;
            }
        }

        public static SourcesViewMode FromUrlParam(string mode)
        {
            switch (mode)
            {
                case "autor":
                    return SourcesViewMode.Author;
                case "nazev":
                    return SourcesViewMode.Name;
                default:
                    return SourcesViewMode.Name;
            }
        }
    }
}