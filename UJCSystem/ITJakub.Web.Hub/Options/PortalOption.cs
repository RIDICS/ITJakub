using System.Collections.Generic;

namespace ITJakub.Web.Hub.Options
{
    public class PortalOption
    {
        public PortalType PortalType { get; set; }

        public PortalType SecondPortalType { get; set; }

        public IDictionary<PortalType, string> PortalUrls { get; set; }

        public string FaviconPath { get; set; }

        public PortalHeadingOption MainPortal { get; set; }

        public PortalHeadingOption SecondPortal { get; set; }

        public Dictionary<string, bool> AllowedSearchOptions { get; set; }
    }
}
