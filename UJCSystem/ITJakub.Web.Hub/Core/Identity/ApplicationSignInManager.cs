using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ITJakub.Web.Hub.Core.Identity
{
    public class ApplicationSignInManager : SignInManager<ApplicationUser>
    {
        public ApplicationSignInManager(UserManager<ApplicationUser> userManager, 
                                        IHttpContextAccessor contextAccessor, 
                                        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, 
                                        IOptions<IdentityOptions> optionsAccessor, 
                                        ILogger<SignInManager<ApplicationUser>> logger,
                                        IAuthenticationSchemeProvider authenticationSchemeProvider) : 
            base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, authenticationSchemeProvider)
        {
            //Should be empty.
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