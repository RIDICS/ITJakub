using Vokabular.Authentication.Client.SharedClient.Exceptions;

namespace Vokabular.Authentication.Client.Exceptions
{
    public class AuthServiceApiException : ServiceApiException
    {
        public AuthServiceApiException(string message) : base(message)
        {
        }
    }
}