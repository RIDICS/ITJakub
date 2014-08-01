using System;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.Core.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace ITJakub.MobileApps.Client.Core.Manager
{
    public class UserManager
    {
        private readonly MobileAppsServiceManager m_manager;

        public UserManager(MobileAppsServiceManager manager)
        {
            m_manager = manager;
        }

        public UserInfo UserInfo { get; private set; }
        public DateTime EstimatedExpirationTime { get; private set; }
        public string CommunicationToken { get; private set; }

        private async Task LoginOauthAsync(LoginProvider loginProvider)
        {
            var loginManager = LoginManager.CreateLoginManager(loginProvider);
            UserInfo = await loginManager.LoginAsync();
        }

        private async Task LoginItJakub(LoginProvider loginProvider)
        {
            var result = await m_manager.LoginUserAsync(loginProvider, UserInfo.Email, UserInfo.AccessToken);
            CommunicationToken = result.CommunicationToken;
            EstimatedExpirationTime = result.EstimatedExpirationTime;
        }

        public async Task<UserInfo> LoginAsync(LoginProvider loginProvider)
        {
            await LoginOauthAsync(loginProvider);
            if (!UserInfo.Success)
                return UserInfo;

            await LoginItJakub(loginProvider);
            
            return UserInfo;
        }

        public async Task<UserInfo> CreateUserAsync(LoginProvider loginProvider)
        {
            await LoginOauthAsync(loginProvider);
            if (!UserInfo.Success)
                return UserInfo;



            await m_manager.CreateUser(loginProvider, UserInfo);//CreateAcc in Manage

            await LoginItJakub(loginProvider);

            return UserInfo;
        }

        
    }
}
