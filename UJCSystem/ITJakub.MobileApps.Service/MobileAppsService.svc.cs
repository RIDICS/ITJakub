using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Service
{
    public class MobileAppsService : IMobileAppsService
    {
        private readonly IMobileAppsService m_manager;

        public MobileAppsService()
        {
            m_manager = Container.Current.Resolve<IMobileAppsService>();
        }

        public string TestMethod(string test)
        {
            return m_manager.TestMethod(test);//Handle exceptions and different managers here
        }
    }
}
