using System;
using Vokabular.Authentication.Client.Storage.Model;

namespace Vokabular.Authentication.Client.Storage
{
    public class InMemoryTokenStorage : ITokenStorage
    {
        private TokenData m_accessToken;

        public TokenData GetAccessToken()
        {
            return m_accessToken;
        }

        public void StoreAccessToken(string accessToken, DateTime accessTokenExpiration)
        {
            m_accessToken = new TokenData{ Token = accessToken, TokenExpiration = accessTokenExpiration};
        }
    }
}