using System;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using ITJakub.MobileApps.Client.Core.Communication.Client;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationProviders
{
    public class ItJakubProvider : ILoginProvider
    {
        private readonly MobileAppsServiceClientManager m_serviceClientManager;

        public ItJakubProvider(MobileAppsServiceClientManager serviceClientManager)
        {
            m_serviceClientManager = serviceClientManager;
        }

        public string AccountName { get { return "It Jakub"; } }

        public AuthProvidersContract ProviderType { get { return AuthProvidersContract.ItJakub; } }

        public async Task<UserLoginSkeleton> LoginAsync()
        {
            var userLoginSkeleton = await LocalAuthentication.LocalAuthenticationBroker.LoginAsync();
            if (!userLoginSkeleton.Success)
                return userLoginSkeleton;


            var client = m_serviceClientManager.GetClient();
            var salt = await client.GetSaltByUserEmail(userLoginSkeleton.Email);

            var passwordHash = GetPasswordHash(userLoginSkeleton.Password, salt);

            var newLoginSkeleton = new UserLoginSkeleton
            {
                AccessToken = passwordHash,
                Email = userLoginSkeleton.Email,
                Success = true
            };

            return newLoginSkeleton;
        }

        public async Task<UserLoginSkeleton> LoginForCreateUserAsync()
        {
            var userLoginSkeleton = await LocalAuthentication.LocalAuthenticationBroker.CreateUserAsync();
            if (!userLoginSkeleton.Success)
                return userLoginSkeleton;

            var newSalt = GenerateSalt();
            var passwordHash = GetPasswordHash(userLoginSkeleton.Password, newSalt);

            userLoginSkeleton.AccessToken = passwordHash;
            userLoginSkeleton.Password = passwordHash;
            userLoginSkeleton.Salt = newSalt;

            return userLoginSkeleton;
        }

        private string GetPasswordHash(string password, string salt)
        {
            var passwordWithSalt = string.Format("{0}{1}", password, salt);

            var hashAlgorithm = HashAlgorithmProvider.OpenAlgorithm("SHA256");
            var binary = CryptographicBuffer.ConvertStringToBinary(passwordWithSalt, BinaryStringEncoding.Utf8);
            var hashed = hashAlgorithm.HashData(binary);
            var resultHash = CryptographicBuffer.EncodeToHexString(hashed);

            return resultHash;
        }

        private string GenerateSalt()
        {
            return Guid.NewGuid().ToString();
        }
    }
}