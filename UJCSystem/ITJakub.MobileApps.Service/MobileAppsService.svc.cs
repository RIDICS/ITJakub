using System;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Service
{
    public class MobileAppsService : IMobileAppsService
    {
        public string TestMethod(string test)
        {
            return string.Format("Hello {0}", test);
        }
    }
}
