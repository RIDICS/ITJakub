using System;

namespace ITJakub.MobileApps.DataContracts
{
    public class AuthException : Exception
    {
        public AuthException(string message):base(message)
        {
        }
    }
}