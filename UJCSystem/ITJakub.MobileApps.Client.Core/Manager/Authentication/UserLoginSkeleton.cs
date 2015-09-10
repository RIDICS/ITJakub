using System;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication
{
    public class UserLoginSkeleton
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public long UserId { get; set; }
        public string CommunicationToken { get; set; }
        public DateTime EstimatedExpirationTime { get; set; }
        public UserRoleContract UserRole { get; set; }
    }

    public class UserLoginSkeletonWithPassword : UserLoginSkeleton
    {
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}