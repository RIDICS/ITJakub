namespace ITJakub.Web.Hub.Helpers
{
    public static class LiteraryOriginalTextConverter
    {
        public static string GetLiteraryOriginalText(string country, string settlement, string repository, string idno, string extent)
        {
            return $"{country}, {settlement}, {repository}, {idno}, {extent}";
        }
    }
}
