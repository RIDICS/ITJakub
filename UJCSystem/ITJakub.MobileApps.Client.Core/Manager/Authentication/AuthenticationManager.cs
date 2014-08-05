using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.ViewModel;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using Microsoft.Practices.Unity;
using Task = System.Threading.Tasks.Task;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication
{
    public class AuthenticationManager
    {
        private readonly Dictionary<LoginProviderType, ILoginProvider> m_loginProviders = new Dictionary<LoginProviderType, ILoginProvider>();

        private readonly MobileAppsServiceManager m_serviceManager;

        public AuthenticationManager(IUnityContainer container)
        {
            m_serviceManager = container.Resolve<MobileAppsServiceManager>();
            LoadLoginProviders(Container.Current.ResolveAll<ILoginProvider>());
        }

        public UserInfo UserInfo { get; private set; }

        private void LoadLoginProviders(IEnumerable<ILoginProvider> providers)
        {
            foreach (ILoginProvider provider in providers)
            {
                m_loginProviders.Add(provider.ProviderType, provider);
            }
        }

        public void GetAllLoginProviderViewModels(Action<List<LoginProviderViewModel>, Exception> callback)
        {
            List<LoginProviderViewModel> viewModels =
                m_loginProviders.Select(x => new LoginProviderViewModel {LoginProviderType = x.Key, Name = x.Value.AccountName}).ToList();
            callback(viewModels, null);
        }

        private async Task LoginItJakubAsync(LoginProviderType loginProviderType)
        {
            LoginResult result = await m_serviceManager.LoginUserAsync(loginProviderType, UserInfo.Email, UserInfo.AccessToken);
            UserInfo.CommunicationToken = result.CommunicationToken;
            UserInfo.EstimatedExpirationTime = result.EstimatedExpirationTime;
            UserInfo.UserId = result.UserId;
            m_serviceManager.UpdateCommunicationToken(result.CommunicationToken);
        }

        public async Task<UserInfo> LoginAsync(LoginProviderType loginProviderType)
        {
            var info = await m_loginProviders[loginProviderType].LoginAsync();
            UserInfo = info;

            if (!info.Success)
                return UserInfo;

            //await LoginItJakubAsync(loginProviderType);
            //TODO HACK for debug
            UserInfo.CommunicationToken = "bfde29d1-d17e-45c2-b9a5-dbfe25be5128";
            UserInfo.UserId = 1;
            m_serviceManager.UpdateCommunicationToken(UserInfo.CommunicationToken);

            return UserInfo;
        }

        public async Task<UserInfo> CreateUserAsync(LoginProviderType loginProviderType)
        {
            var info = await m_loginProviders[loginProviderType].LoginAsync();
            UserInfo = info;

            if (!info.Success)
                return UserInfo;

            await m_serviceManager.CreateUser(loginProviderType, info);
            await LoginItJakubAsync(loginProviderType);

            return UserInfo;
        }

        public void LogOut()
        {
            UserInfo.AccessToken = string.Empty;
            UserInfo.CommunicationToken = string.Empty;
            UserInfo = null;
            m_serviceManager.UpdateCommunicationToken(string.Empty);
        }
    }
}