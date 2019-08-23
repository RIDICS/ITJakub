namespace ITJakub.Web.Hub.Options
{
    public class AutoLoginCookieConfiguration
    {
        public const string CookieNamePrefix = "AutoLoginAttempted";

        public const string CookieValue = "true";

        public int ExpirationTimeInSeconds { get; set; }

        public string LoginCheckPath { get; set; }

        public string CookieName { get; set; }
    }
}
