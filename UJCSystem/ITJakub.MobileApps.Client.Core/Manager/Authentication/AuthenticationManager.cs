using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.Communication.Client;
using ITJakub.MobileApps.Client.Core.Communication.Error;
using ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationProviders;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.DataContracts;
using Microsoft.Practices.Unity;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication
{
    public class AuthenticationManager
    {
        private readonly Dictionary<AuthProvidersContract, ILoginProvider> m_loginProviders = new Dictionary<AuthProvidersContract, ILoginProvider>();
        private readonly UserAvatarCache m_userAvatarCache;
        private readonly MobileAppsServiceClientManager m_serviceClientManager;
        private readonly ConfigurationManager m_configurationManager;

        public UserLoginSkeleton UserLoginInfo { get; set; }

        public AuthenticationManager(IUnityContainer container)
        {
            m_serviceClientManager = container.Resolve<MobileAppsServiceClientManager>();
            m_userAvatarCache = container.Resolve<UserAvatarCache>();
            m_configurationManager = container.Resolve<ConfigurationManager>();
            LoadLoginProviders(container.ResolveAll<ILoginProvider>());
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
                m_loginProviders.Select(
                    x => new LoginProviderViewModel {LoginProviderType = x.Key, Name = x.Value.AccountName}).ToList();
            callback(viewModels, null);
        }

        private async Task LoginItJakubAsync(AuthProvidersContract loginProviderType,
            UserLoginSkeleton userLoginSkeleton)
        {
            var client = m_serviceClientManager.GetClient();

            var response =
                await client.LoginUserAsync(loginProviderType, userLoginSkeleton.AccessToken, userLoginSkeleton.Email);

            userLoginSkeleton.CommunicationToken = response.CommunicationToken;
            userLoginSkeleton.EstimatedExpirationTime = response.EstimatedExpirationTime;
            userLoginSkeleton.UserId = response.UserId;
            userLoginSkeleton.UserRole = response.UserRole;
            userLoginSkeleton.Email = response.Email;
            userLoginSkeleton.FirstName = response.FirstName;
            userLoginSkeleton.LastName = response.LastName;

            m_userAvatarCache.AddAvatarUrl(response.UserId, response.ProfilePictureUrl);
            m_serviceClientManager.UpdateCommunicationToken(response.CommunicationToken);
        }

        private async Task<UserLoginSkeleton> LoginAsync(AuthProvidersContract loginProviderType)
        {
            UserLoginSkeleton loginSkeleton = await m_loginProviders[loginProviderType].LoginAsync();

            while (true)
            {
                var isUserNotRegisteredError = false;
                var isItjLoginError = false;

                if (!loginSkeleton.Success)
                    return loginSkeleton;

                try
                {
                    await LoginItJakubAsync(loginProviderType, loginSkeleton);
                }
                catch (UserNotRegisteredException)
                {
                    isUserNotRegisteredError = true;
                    if (loginProviderType == AuthProvidersContract.ItJakub)
                        isItjLoginError = true;
                }

                if (isUserNotRegisteredError)
                {
                    if (isItjLoginError)
                    {
                        loginSkeleton = await m_loginProviders[loginProviderType].ReopenWithErrorAsync();
                        continue;
                    }

                    try
                    {
                        await CreateUserItJakubAsync(loginProviderType, loginSkeleton);
                    }
                    catch (UserAlreadyRegisteredException ex)
                    {
                        throw new ClientCommunicationException(ex);
                    }
                }

                return loginSkeleton;
            }
        }


        private async Task CreateUserItJakubAsync(AuthProvidersContract loginProviderType,
            UserLoginSkeleton loginSkeleton)
        {
            UserDetailContract userDetail;
            if (loginSkeleton is UserLoginSkeletonWithPassword)
            {
                var loginSkeletonWithPassword = (UserLoginSkeletonWithPassword) loginSkeleton;

                userDetail = new PasswordUserDetailContract
                {
                    PasswordHash = loginSkeletonWithPassword.Password,
                    PasswordSalt = loginSkeletonWithPassword.Salt
                };
            }
            else
            {
                userDetail = new UserDetailContract();
            }

            userDetail.Email = loginSkeleton.Email;
            userDetail.FirstName = loginSkeleton.FirstName;
            userDetail.LastName = loginSkeleton.LastName;


            var client = m_serviceClientManager.GetClient();

            await client.CreateUserAsync(loginProviderType, loginSkeleton.AccessToken, userDetail);

            await LoginItJakubAsync(loginProviderType, loginSkeleton);
        }

        private async Task<UserLoginSkeleton> CreateUserAsync(AuthProvidersContract loginProviderType)
        {
            UserLoginSkeleton loginSkeleton = await m_loginProviders[loginProviderType].LoginForCreateUserAsync();

            while (true)
            {
                var isItjCreateUserError = false;

                if (!loginSkeleton.Success)
                    return loginSkeleton;

                try
                {
                    await CreateUserItJakubAsync(loginProviderType, loginSkeleton);
                }
                catch (UserAlreadyRegisteredException)
                {
                    if (loginProviderType != AuthProvidersContract.ItJakub)
                        throw;

                    isItjCreateUserError = true;
                }

                if (isItjCreateUserError)
                {
                    loginSkeleton = await m_loginProviders[loginProviderType].ReopenWithErrorAsync();
                    continue;
                }

                return loginSkeleton;
            }
        }

        public void LogOut()
        {
            UserLoginInfo = null;
            m_serviceClientManager.Logout();
        }

        public async void LoginByProvider(AuthProvidersContract loginProviderType, Action<bool, Exception> callback)
        {
            try
            {
                await m_configurationManager.UpdateBookLibraryEndpointAddress();
                UserLoginSkeleton userDetail = await LoginAsync(loginProviderType);
                UserLoginInfo = userDetail.Success ? userDetail : null;
                callback(userDetail.Success, null);
            }
            catch (UserNotRegisteredException exception)
            {
                callback(false, exception);
            }
            catch (InvalidServerOperationException exception)
            {
                callback(false, exception);
            }
            catch (ClientCommunicationException exception)
            {
                callback(false, exception);
            }
        }

        public async void CreateUserByLoginProvider(AuthProvidersContract loginProviderType,
            Action<bool, Exception> callback)
        {
            try
            {
                await m_configurationManager.UpdateBookLibraryEndpointAddress();
                UserLoginSkeleton userLoginSkeleton = await CreateUserAsync(loginProviderType);
                UserLoginInfo = userLoginSkeleton.Success ? userLoginSkeleton : null;
                callback(userLoginSkeleton.Success, null);
            }
            catch (UserAlreadyRegisteredException exception)
            {
                callback(false, exception);
            }
            catch (InvalidServerOperationException exception)
            {
                callback(false, exception);
            }
            catch (ClientCommunicationException exception)
            {
                callback(false, exception);
            }
        }

        public async void PromoteUserToTeacherRole(long userId, string promotionCode, Action<bool, Exception> callback)
        {
            try
            {
                var client = m_serviceClientManager.GetClient();
                var result = await client.PromoteUserToTeacherRoleAsync(userId, promotionCode);

                callback(result, null);
            }
            catch (InvalidServerOperationException exception)
            {
                callback(false, exception);
            }
            catch (ClientCommunicationException exception)
            {
                callback(false, exception);
            }
        }

        public async void GetLoggedUserInfo(bool getUserAvatar, Action<LoggedUserViewModel> callback)
        {
            if (UserLoginInfo == null)
            {
                callback(null);
                return;
            }

            var viewModel = GetLoggedUserViewModel();
            callback(viewModel);

            if (getUserAvatar)
            {
                viewModel.UserAvatar = await m_userAvatarCache.GetUserAvatar(UserLoginInfo.UserId);
                callback(viewModel);
            }
        }

        private LoggedUserViewModel GetLoggedUserViewModel()
        {
            var viewModel = new LoggedUserViewModel
            {
                UserId = UserLoginInfo.UserId,
                FirstName = UserLoginInfo.FirstName,
                LastName = UserLoginInfo.LastName,
                Email = UserLoginInfo.Email,
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