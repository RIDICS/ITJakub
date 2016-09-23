using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ITJakub.Web.Hub.Identity
{
    public class ApplicationSignInManager : SignInManager<ApplicationUser>
    {
        public ApplicationSignInManager(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<ApplicationUser>> logger) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger)
        {
        }
        
        //public async override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        //{
        //    return await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes. ApplicationCookie);            
        //}

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            return await base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
        }
    }
}