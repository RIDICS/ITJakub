namespace ITJakub.MVCWebLayer.Enums
{
    public static class SourcesViewModeConverter
    {

        public static string ToUrlParam(this SourcesViewType type)
        {
            switch (type)
            {
                case SourcesViewType.Author:
                    return "autor";
                case SourcesViewType.Name:
                    return "nazev";
                default:
                    return string.Empty;
            }
        }

        public static SourcesViewType FromUrlParam(string mode)
        {
            switch (mode)
            {
                case "autor":
                    return SourcesViewType.Author;
                case "nazev":
                    return SourcesViewType.Name;
                default:
                    return SourcesViewType.Name;
            }
        }
    }
}