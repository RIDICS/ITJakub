using System.Collections.Generic;

namespace ITJakub.Web.Hub.Options
{
    public class PortalOption
    {
        public PortalType PortalType { get; set; }

        public string SecondPortalUrl { get; set; }

        public string FaviconPath { get; set; }

        public PortalConfig MainPortal { get; set; }

        public PortalConfig SecondPortal { get; set; }

        public Dictionary<string, bool> AllowedSearchOptions { get; set; }
    }
}
