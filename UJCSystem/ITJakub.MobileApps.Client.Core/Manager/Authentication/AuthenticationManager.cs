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

        private readonly MobileAppsServiceManager m_manager;

        public AuthenticationManager(IUnityContainer container)
        {
            m_manager = container.Resolve<MobileAppsServiceManager>();
            LoadLoginProviders(Container.Current.ResolveAll<ILoginProvider>());

            //TODO HACK for debug
            CommunicationToken = "ab617d8f-b6bc-44c3-87a9-e38f808039af";
        }

        public UserInfo UserInfo { get; private set; }
        public DateTime EstimatedExpirationTime { get; private set; }
        public string CommunicationToken { get; private set; }

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
            LoginResult result = await m_manager.LoginUserAsync(loginProviderType, UserInfo.Email, UserInfo.AccessToken);
            CommunicationToken = result.CommunicationToken;
            EstimatedExpirationTime = result.EstimatedExpirationTime;
        }

        public async Task<UserInfo> LoginAsync(LoginProviderType loginProviderType)
        {
            var info = await m_loginProviders[loginProviderType].LoginAsync();
            UserInfo = info;

            if (!info.Success)
                return UserInfo;

            //await LoginItJakubAsync(loginProviderType);

            return UserInfo;
        }

        public async Task<UserInfo> CreateUserAsync(LoginProviderType loginProviderType)
        {
            var info = await m_loginProviders[loginProviderType].LoginAsync();
            UserInfo = info;

            if (!info.Success)
                return UserInfo;

            await m_manager.CreateUser(loginProviderType, info);
            await LoginItJakubAsync(loginProviderType);

            return UserInfo;
        }

        public void LogOut()
        {
            UserInfo = null;
            EstimatedExpirationTime = new DateTime();
            CommunicationToken = null;
        }
    }
}