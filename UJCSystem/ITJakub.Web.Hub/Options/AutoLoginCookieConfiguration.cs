namespace ITJakub.Web.Hub.Options
{
    public class AutoLoginCookieConfiguration
    {
        public const string Name = "AutoLoginAttempted";

        public const string Value = "true";

        public int ExpirationTimeInSeconds { get; set; }
    }
}
