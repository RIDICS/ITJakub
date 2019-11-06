namespace ITJakub.Web.Hub.Helpers
{
    public static class LiteraryOriginalTextConverter
    {
        public static string GetLiteraryOriginalText(string country, string settlement, string repository, string idno, string extent)
        {
            if (!string.IsNullOrEmpty(country)
                || !string.IsNullOrEmpty(settlement)
                || !string.IsNullOrEmpty(repository)
                || !string.IsNullOrEmpty(idno)
                || !string.IsNullOrEmpty(extent))
            {
                return $"{country}, {settlement}, {repository}, {idno}, {extent}";
            }

            return string.Empty;
        }
    }
}