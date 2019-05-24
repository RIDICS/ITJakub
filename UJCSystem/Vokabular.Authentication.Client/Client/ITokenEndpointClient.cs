using System.Threading.Tasks;
using IdentityModel.Client;

namespace Vokabular.Authentication.Client.Client
{
    public interface ITokenEndpointClient
    {
        Task<TokenResponse> GetAccessTokenAsync(string scope);
        Task<TokenResponse> RefreshTokenAsync(string refreshToken);
        Task<TokenRevocationResponse> RevokeTokenAsync(string refreshToken);
    }
}