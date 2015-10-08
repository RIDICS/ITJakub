using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.ServiceModel.Security.Tokens;
using System.Threading.Tasks;
using ITJakub.Shared.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace ITJakub.Web.Hub.Identity
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        private readonly CommunicationProvider m_communication = new CommunicationProvider();

        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new ApplicationUserStore());

            manager.PasswordHasher = new PasswordHasher();
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 1,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = false;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            //{
            //    MessageFormat = "Your security code is {0}"
            //});
            //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            //{
            //    Subject = "Security Code",
            //    BodyFormat = "Your security code is {0}"
            //});
            //manager.EmailService = new EmailService();            

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public async override Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            var isValid = await base.CheckPasswordAsync(user, password);
            return isValid;
        }

        public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return base.CreateAsync(user, password);
        }

        public async override Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var result = await base.FindByNameAsync(userName);
            return result;

        }

        public bool RenewCommunicationToken(string userName, string password)
        {
            using (var client = m_communication.GetEncryptedClient())
            {
                return client.RenewCommToken(userName, password);
            }
            
        }

        public async override Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user, string authenticationType)
        {

            //IList<SpecialPermissionContract> specialPermissions;
            using (var authenticatedClient = m_communication.GetAuthenticatedClient(user.UserName, user.CommunicationToken))
            {
                user.SpecialPermissions = authenticatedClient.GetSpecialPermissionsForUserByType(SpecialPermissionCategorizationEnumContract.Action);
            }

            var claimsIdentity = await base.CreateIdentityAsync(user, authenticationType);
            claimsIdentity.AddClaim(new Claim(CustomClaimType.CommunicationToken, user.CommunicationToken));

            if (user.SpecialPermissions.Count != 0)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, CustomRole.CanViewAdminModule));
            }

            if (user.SpecialPermissions.OfType<UploadBookPermissionContract>().Count() != 0)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, CustomRole.CanUploadBooks));
            }

            if (user.SpecialPermissions.OfType<NewsPermissionContract>().Count() != 0)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, CustomRole.CanAddNews));
            }

            if (user.SpecialPermissions.OfType<ManagePermissionsPermissionContract>().Count() != 0)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, CustomRole.CanManagePermissions));
            }

            if (user.SpecialPermissions.OfType<FeedbackPermissionContract>().Count() != 0)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, CustomRole.CanManageFeedbacks));
            }

            return claimsIdentity;
        }
    }   
}