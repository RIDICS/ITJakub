namespace ITJakub.Web.Hub.Options
{
    public static class PortalOptionExtensions
    {
        public static string GetSecondPortalUrl(this PortalOption portalOption)
        {
            return portalOption.PortalUrls[portalOption.SecondPortalType];
        }
    }
}