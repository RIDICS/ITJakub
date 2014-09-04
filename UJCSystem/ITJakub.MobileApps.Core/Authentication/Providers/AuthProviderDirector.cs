using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Core.Authentication.Providers
{
    public class AuthProviderDirector
    {
        private readonly Dictionary<AuthProvidersContract,IAuthProvider> m_authProviders=new Dictionary<AuthProvidersContract, IAuthProvider>();

        public AuthProviderDirector(IKernel container)
        {
            var providers = container.ResolveAll<IAuthProvider>();
            LoadAuthProviders(providers);
        }

        private void LoadAuthProviders(IEnumerable<IAuthProvider> providers)
        {
            foreach (var provider in providers)
            {
                m_authProviders.Add(provider.ProviderContractType,provider);
            }
        }

        public IAuthProvider GetProvider(AuthProvidersContract providerContractType)
        {
            if(!m_authProviders.ContainsKey(providerContractType)) 
                throw new ArgumentException(string.Format("This type: '{0}' of providerContract does not exist!",providerContractType));

            return m_authProviders[providerContractType];
        }
    }
}