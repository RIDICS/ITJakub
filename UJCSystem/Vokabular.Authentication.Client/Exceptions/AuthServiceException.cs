using System;
using Vokabular.Authentication.Client.SharedClient.Exceptions;

namespace Vokabular.Authentication.Client.Exceptions
{
    public class AuthServiceException : ServiceException
    {
        public AuthServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}