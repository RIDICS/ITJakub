using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Core.Authentication.Providers
{
    public class AuthProviderDirector
    {
        private readonly Dictionary<AuthenticationProviders,IAuthProvider> m_authProviders=new Dictionary<AuthenticationProviders, IAuthProvider>();

        public AuthProviderDirector(IKernel container)
        {
            var providers = container.ResolveAll<IAuthProvider>();
            LoadAuthProviders(providers);
        }

        private void LoadAuthProviders(IEnumerable<IAuthProvider> providers)
        {
            foreach (var provider in providers)
            {
                m_authProviders.Add(provider.ProviderType,provider);
            }
        }

        public IAuthProvider GetProvider(AuthenticationProviders providerType)
        {
            if(!m_authProviders.ContainsKey(providerType)) 
                throw new ArgumentException(string.Format("This type: '{0}' of provider does not exist!",providerType));

            return m_authProviders[providerType];
        }
    }
}