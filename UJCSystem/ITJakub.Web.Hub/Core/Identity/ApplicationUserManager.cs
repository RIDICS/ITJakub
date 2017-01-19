using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ITJakub.Web.Hub.Core.Identity
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        private readonly CommunicationProvider m_communication;

        public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger, CommunicationProvider communicationProvider)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            m_communication = communicationProvider;
        }

        // Before ASP.NET Core:

        //public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        //{
        //    var manager = new ApplicationUserManager(new ApplicationUserStore());
        //    manager.PasswordHasher = new CustomPasswordHasher();
        //    // Configure validation logic for usernames
        //    manager.UserValidator = new UserValidator<ApplicationUser>(manager)
        //    {
        //        AllowOnlyAlphanumericUserNames = false,
        //        RequireUniqueEmail = false
        //    };

        //    // Configure validation logic for passwords
        //    manager.PasswordValidator = new PasswordValidator
        //    {
        //        RequiredLength = 1,
        //        RequireNonLetterOrDigit = false,
        //        RequireDigit = false,
        //        RequireLowercase = false,
        //        RequireUppercase = false,
        //    };

        //    // Configure user lockout defaults
        //    manager.UserLockoutEnabledByDefault = false;
        //    manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
        //    manager.MaxFailedAccessAttemptsBeforeLockout = 5;
        //    // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
        //    // You can write your own provider and plug it in here.
        //    //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
        //    //{
        //    //    MessageFormat = "Your security code is {0}"
        //    //});
        //    //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
        //    //{
        //    //    Subject = "Security Code",
        //    //    BodyFormat = "Your security code is {0}"
        //    //});
        //    //manager.EmailService = new EmailService();            

        //    var dataProtectionProvider = options.DataProtectionProvider;
        //    if (dataProtectionProvider != null)
        //    {
        //        manager.UserTokenProvider =
        //            new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
        //    }
        //    return manager;
        //}

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

        public bool RenewCommunicationToken(string userName)
        {
            using (var client = m_communication.GetEncryptedClient())
            {
                return client.RenewCommToken(userName);
            }
        }
    }
}