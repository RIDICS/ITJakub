using System;

namespace ITJakub.MobileApps.Client.Core.Manager.Communication.Error
{
    public class UserNotRegisteredException : Exception
    {
        public UserNotRegisteredException()
        {
        }

        public UserNotRegisteredException(string message) : base(message)
        {
        }

        public UserNotRegisteredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UserNotRegisteredException(Exception innerException) : base("User isn't registered.", innerException)
        {
        }
    }
}