using ITJakub.MobileApps.Core.Authentication.Image;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataEntities.Database.Repositories;
using User = ITJakub.MobileApps.DataEntities.Database.Entities.User;

namespace ITJakub.MobileApps.Core.Authentication.Providers
{
    public class ItJakubAuthProvider : IAuthProvider
    {
        private readonly GravatarImageUrlProvider m_imageUrlProvider;
        private readonly UsersRepository m_usersRepository;

        public ItJakubAuthProvider(UsersRepository usersRepository, GravatarImageUrlProvider imageUrlProvider)
        {
            m_usersRepository = usersRepository;
            m_imageUrlProvider = imageUrlProvider;
        }

        public bool IsExternalProvider
        {
            get { return false; }
        }

        public AuthenticationProviders ProviderType
        {
            get { return AuthenticationProviders.ItJakub; }
        }

        public AuthenticateResultInfo Authenticate(UserLogin userLogin, User dbUser)
        {
            return Authenticate(userLogin.AuthenticationToken, dbUser.Email);
        }

        private AuthenticateResultInfo Authenticate(string passwordHash, string email)
        {
            User user = m_usersRepository.FindByEmailAndProvider(email, (byte) AuthenticationProviders.ItJakub);
            bool authSucceeded = user.PasswordHash.Equals(passwordHash);
            var result = new AuthenticateResultInfo
            {
                Result = authSucceeded ? AuthResultType.Success : AuthResultType.Failed,
            };

            if (authSucceeded)
                result.UserImageLocation = GetImageLocation(email);
            return result;
        }

        private string GetImageLocation(string email)
        {
            return m_imageUrlProvider.GetImageUrl(email);
        }
    }
}