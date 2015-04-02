using System;

namespace ITJakub.MobileApps.Client.Core.Manager.Communication.Error
{
    public class UserAlreadyRegisteredException : Exception
    {
        public UserAlreadyRegisteredException()
        {
        }

        public UserAlreadyRegisteredException(string message) : base(message)
        {
        }

        public UserAlreadyRegisteredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UserAlreadyRegisteredException(Exception innerException) : base("User is already registered.", innerException)
        {
        }
    }
}