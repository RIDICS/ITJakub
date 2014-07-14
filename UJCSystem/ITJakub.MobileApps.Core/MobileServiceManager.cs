using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Core
{
    public class MobileServiceManager : IMobileAppsService
    {
        public string TestMethod(string test)
        {
            return string.Format("Hello {0}", test);
        }
    }
}