using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationProviders;
using ITJakub.MobileApps.Client.Core.Manager.Communication;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.DataContracts;
using Microsoft.Practices.Unity;
using Task = System.Threading.Tasks.Task;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication
{
    public class AuthenticationManager
    {
        private readonly Dictionary<AuthProvidersContract, ILoginProvider> m_loginProviders = new Dictionary<AuthProvidersContract, ILoginProvider>();

        private readonly MobileAppsServiceManager m_serviceManager;
        private readonly UserAvatarCache m_userAvatarCache;

        private UserLoginSkeleton UserLoginInfo { get; set; }

        public AuthenticationManager(IUnityContainer container)
        {
            m_serviceManager = container.Resolve<MobileAppsServiceManager>();
            m_userAvatarCache = container.Resolve<UserAvatarCache>();
            LoadLoginProviders(Container.Current.ResolveAll<ILoginProvider>());
        }
        
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

        private async Task LoginItJakubAsync(AuthProvidersContract loginProviderType)
        {
            LoginResult result = await m_serviceManager.LoginUserAsync(loginProviderType, UserLoginInfo.Email, UserLoginInfo.AccessToken);
            UserLoginInfo.CommunicationToken = result.CommunicationToken;
            UserLoginInfo.EstimatedExpirationTime = result.EstimatedExpirationTime;
            UserLoginInfo.UserId = result.UserId;
            UserLoginInfo.UserRole = result.UserRole;

            m_userAvatarCache.AddAvatarUrl(result.UserId, result.UserAvatarUrl);
            m_serviceManager.UpdateCommunicationToken(result.CommunicationToken);
        }

        private async Task<UserLoginSkeleton> LoginAsync(AuthProvidersContract loginProviderType)
        {
            UserLoginSkeleton loginSkeleton = await m_loginProviders[loginProviderType].LoginAsync();
            UserLoginInfo = loginSkeleton;

            if (!loginSkeleton.Success)
                return UserLoginInfo;

            await LoginItJakubAsync(loginProviderType);

            m_serviceManager.UpdateCommunicationToken(UserLoginInfo.CommunicationToken);

            return UserLoginInfo;
        }

        private async Task<UserLoginSkeleton> CreateUserAsync(AuthProvidersContract loginProviderType)
        {
            UserLoginSkeleton loginSkeleton = await m_loginProviders[loginProviderType].LoginAsync();
            UserLoginInfo = loginSkeleton;

            if (!loginSkeleton.Success)
                return UserLoginInfo;

            await m_serviceManager.CreateUserAsync(loginProviderType, loginSkeleton); //TODO ZAROVEN create user a zaroven login ? tak to fungovat nebude....
            await LoginItJakubAsync(loginProviderType);

            return UserLoginInfo;
        }

        public void LogOut()
        {
            UserLoginInfo = null;
            m_serviceManager.UpdateCommunicationToken(string.Empty);
        }

        public async void LoginByProvider(AuthProvidersContract loginProviderType, Action<bool, Exception> callback)
        {
            try
            {
                UserLoginSkeleton userDetail = await LoginAsync(loginProviderType);
                callback(userDetail.Success, null);
            }
            catch (UserNotRegisteredException exception)
            {
                callback(false, exception);
            }
            catch (ClientCommunicationException exception)
            {
                callback(false, exception);
            }
        }

        public async void CreateUserByLoginProvider(AuthProvidersContract loginProviderType, Action<bool, Exception> callback)
        {
            try
            {
                UserLoginSkeleton userLoginSkeleton = await CreateUserAsync(loginProviderType);//TODO spravit bug... registrace a login nemohou jet naraz v ruznych vlaknech, protoze pak to obcas spadne na user not authorized exception
                callback(userLoginSkeleton.Success, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(false, exception);
            }
        }

        public async void GetLoggedUserInfo(Action<LoggedUserViewModel, Exception> callback)
        {
            var viewModel = GetLoggedUserViewModel();
            callback(viewModel, null);
            viewModel.UserAvatar = await m_userAvatarCache.GetUserAvatar(UserLoginInfo.UserId);
            callback(viewModel, null);
        }

        private LoggedUserViewModel GetLoggedUserViewModel()
        {
            var viewModel = new LoggedUserViewModel
            {
                FirstName = UserLoginInfo.FirstName,
                LastName = UserLoginInfo.LastName,
                UserRole = UserLoginInfo.UserRole
            };
            return viewModel;
        }

        public long? GetCurrentUserId()
        {
            return UserLoginInfo == null ? (long?) null : UserLoginInfo.UserId;
        }

    }
}