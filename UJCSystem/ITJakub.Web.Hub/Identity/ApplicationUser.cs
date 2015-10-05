using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ITJakub.Shared.Contracts;
using Microsoft.AspNet.Identity;

namespace ITJakub.Web.Hub.Identity
{
    public class ApplicationUser : IUser<string>
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreateTime { get; set; }
        public string CommunicationToken { get; set; }
        public IList<SpecialPermissionContract> SpecialPermissions { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
}