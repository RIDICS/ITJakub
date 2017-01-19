namespace ITJakub.Web.Hub.Core
{
    public static class Favorites
    {
        public const int MaxTitleLength = 250;
        public const int MaxLabelLength = 50;

        public static string ShortenTitle(string title)
        {
            const string ellipsis = "...";
            return title.Length > MaxTitleLength
                ? string.Format("{0}{1}", title.Substring(0, MaxTitleLength - ellipsis.Length), ellipsis)
                : title;
        }
    }
}