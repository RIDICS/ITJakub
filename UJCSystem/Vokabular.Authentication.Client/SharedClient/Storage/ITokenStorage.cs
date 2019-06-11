using System;
using Vokabular.Authentication.Client.SharedClient.Storage.Model;

namespace Vokabular.Authentication.Client.SharedClient.Storage
{
    public interface ITokenStorage
    {
        TokenData GetAccessToken();
        void StoreAccessToken(string accessToken, DateTime accessTokenExpiration);
    }
}