using System.Collections.Generic;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.ViewModel;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication
{
    public abstract class IAuthProvider
    {
        public static IAuthProvider CreateLoginManager(LoginProvider loginProvider)
        {
            switch (loginProvider)
            {
                case LoginProvider.LiveId:
                    return new LiveIdManager();
                case LoginProvider.Facebook:
                    return new FacebookManager();
                case LoginProvider.Google:
                    return new GoogleManager();
                default:
                    return null;
            }
        }

        public abstract Task<UserInfo> LoginAsync();
    }

    public class AuthProviderDirector
    {
        private readonly Dictionary<LoginProvider, IAuthProvider> m_authProviders = new Dictionary<LoginProvider, IAuthProvider>();

        public AuthProviderDirector()
        {
            LoadLoginProviders();
        }

        private void LoadLoginProviders()
        {
            //todo inject from IOC
            m_authProviders.Add();
        }
    }
}
