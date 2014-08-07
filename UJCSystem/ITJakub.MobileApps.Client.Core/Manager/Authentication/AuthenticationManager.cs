﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.Error;
using ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationProviders;
using ITJakub.MobileApps.Client.Core.ViewModel.Authentication;
using Microsoft.Practices.Unity;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication
{
    public class AuthenticationManager
    {
        private readonly Dictionary<LoginProviderType, ILoginProvider> m_loginProviders = new Dictionary<LoginProviderType, ILoginProvider>();

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

        private async Task LoginItJakubAsync(LoginProviderType loginProviderType, Action<UserLoginSkeleton, Exception> callback)
        {
            LoginResult result = await m_serviceManager.LoginUserAsync(loginProviderType, UserLoginInfo.Email, UserLoginInfo.AccessToken);
            UserLoginInfo.CommunicationToken = result.CommunicationToken;
            UserLoginInfo.EstimatedExpirationTime = result.EstimatedExpirationTime;
            UserLoginInfo.UserId = result.UserId;
            UserLoginInfo.IsTeacher = result.IsTeacher;

            m_userAvatarCache.AddAvatarUrl(result.UserId, result.UserAvatarUrl);
            m_serviceManager.UpdateCommunicationToken(result.CommunicationToken);
        }

        private async Task<UserLoginSkeleton> LoginAsync(LoginProviderType loginProviderType, Action<UserLoginSkeleton, Exception> callback)
        {
            UserLoginSkeleton loginSkeleton = await m_loginProviders[loginProviderType].LoginAsync();
            UserLoginInfo = loginSkeleton;

            if (!loginSkeleton.Success)
                return UserLoginInfo;

            await LoginItJakubAsync(loginProviderType, callback);

            m_serviceManager.UpdateCommunicationToken(UserLoginInfo.CommunicationToken);

            return UserLoginInfo;
        }

        private async Task<UserLoginSkeleton> CreateUserAsync(LoginProviderType loginProviderType, Action<UserLoginSkeleton, Exception> callback)
        {
            UserLoginSkeleton loginSkeleton = await m_loginProviders[loginProviderType].LoginAsync();
            UserLoginInfo = loginSkeleton;

            if (!loginSkeleton.Success)
                return UserLoginInfo;

            await m_serviceManager.CreateUser(loginProviderType, loginSkeleton); //TODO ZAROVEN create user a zaroven login ? tak to fungovat nebude....
            await LoginItJakubAsync(loginProviderType, callback);

            return UserLoginInfo;
        }

        public void LogOut()
        {
            UserLoginInfo.AccessToken = string.Empty; // TODO WTF tyhle radky ?
            UserLoginInfo.CommunicationToken = string.Empty; // TODO WTF tyhle radky ?
            UserLoginInfo = null;
            m_serviceManager.UpdateCommunicationToken(string.Empty);
        }

        public async void LoginByProvider(LoginProviderType loginProviderType, Action<UserLoginSkeleton, Exception> callback)
        {
            try
            {
                UserLoginSkeleton userDetail = await LoginAsync(loginProviderType, callback);
            }
            catch (UserNotRegisteredException exception)
            {
                callback(null, exception);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void CreateUserByLoginProvider(LoginProviderType loginProviderType, Action<UserLoginSkeleton, Exception> callback)
        {
            try
            {
                UserLoginSkeleton userLoginSkeleton = await CreateUserAsync(loginProviderType, callback);//TODO spravit bug... registrace a login nemohou jet naraz v ruznych vlaknech, protoze pak to obcas spadne na user not authorized exception
                callback(userLoginSkeleton, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }

        public async void GetLogedUserInfo(Action<LogedUserViewModel, Exception> callback)
        {
            var viewModel = new LogedUserViewModel
            {
                FirstName = UserLoginInfo.FirstName,
                LastName = UserLoginInfo.LastName,
            };
            callback(viewModel, null);
            viewModel.UserAvatar = await m_userAvatarCache.GetUserAvatar(UserLoginInfo.UserId);
        }

        public long? GetCurrentUserId()
        {
            return UserLoginInfo == null ? (long?) null : UserLoginInfo.UserId;
        }

    }
}