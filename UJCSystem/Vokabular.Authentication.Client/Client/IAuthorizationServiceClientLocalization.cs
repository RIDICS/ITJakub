using Vokabular.Authentication.Client.Exceptions;

namespace Vokabular.Authentication.Client.Client
{
    public interface IAuthorizationServiceClientLocalization
    {
        void LocalizeApiException(AuthServiceApiException ex);
        string GetCurrentCulture();
    }
}