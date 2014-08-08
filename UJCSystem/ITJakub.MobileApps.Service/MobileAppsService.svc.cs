using System.Reflection;
using System.ServiceModel.Web;
using ITJakub.MobileApps.Core;
using ITJakub.MobileApps.DataContracts;
using log4net;

namespace ITJakub.MobileApps.Service
{
    public class MobileAppsService : IMobileAppsService
    {
        private readonly IMobileAppsService m_serviceManager;
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MobileAppsService()
        {
            m_serviceManager = Container.Current.Resolve<IMobileAppsService>();
        }

        public void CreateUser(AuthProvidersContract providerContract, string providerToken, UserDetailContract userDetail)
        {
            try
            {
                m_serviceManager.CreateUser(providerContract, providerToken, userDetail);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }

        public LoginUserResponse LoginUser(AuthProvidersContract providerContract, string providerToken, string email)
        {
            try
            {
                return m_serviceManager.LoginUser(providerContract, providerToken, email);
            }
            catch (WebFaultException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(ex.Message);

                throw;
            }
        }
    }
}