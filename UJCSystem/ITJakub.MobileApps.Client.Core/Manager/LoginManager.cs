using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.ViewModel;

namespace ITJakub.MobileApps.Client.Core.Manager
{
    public abstract class LoginManager
    {
        public static LoginManager CreateLoginManager(LoginProvider loginProvider)
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
}
