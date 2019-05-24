using System;
using Vokabular.Authentication.Client.Model;

namespace Vokabular.Authentication.Client.Storage
{
    public interface ITokenStorage
    {
        TokenData GetAccessToken();
        void StoreAccessToken(string accessToken, DateTime accessTokenExpiration);
    }
}