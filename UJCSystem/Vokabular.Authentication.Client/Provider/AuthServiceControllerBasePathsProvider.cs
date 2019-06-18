using System;
using System.Collections.Generic;
using Vokabular.Authentication.Client.Configuration;
using Vokabular.Authentication.DataContracts;
using Vokabular.Authentication.DataContracts.User;

namespace Vokabular.Authentication.Client.Provider
{
    public class AuthServiceControllerBasePathsProvider
    {
        private readonly Dictionary<Type, string> m_basePaths;

        public AuthServiceControllerBasePathsProvider(AuthServiceControllerBasePathsConfiguration basePathsConfiguration)
        {
            m_basePaths = new Dictionary<Type, string>
            {
                {typeof(PermissionContract), basePathsConfiguration.PermissionBasePath},
                {typeof(RoleContract), basePathsConfiguration.RoleBasePath},
                {typeof(UserContractBase), basePathsConfiguration.UserBasePath},
                {typeof(BasicUserInfoContract), basePathsConfiguration.UserBasePath},
                {typeof(UserContract), basePathsConfiguration.UserBasePath},
                {typeof(CreateUserContract), basePathsConfiguration.RegistrationBasePath},
                {typeof(ExternalLoginProviderContract), basePathsConfiguration.ExternalLoginProviderBasePath},
            };
        }

        public string GetControllerBasePath<TContract>()
        {
            return m_basePaths[typeof(TContract)];
        }
    }
}