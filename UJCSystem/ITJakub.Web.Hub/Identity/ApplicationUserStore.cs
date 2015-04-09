using System;
using System.Threading.Tasks;
using System.Web.WebPages;
using Microsoft.AspNet.Identity;

namespace ITJakub.Web.Hub.Identity
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    // Configure the application sign-in manager which is used in this application.

    public class ApplicationUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserLockoutStore<ApplicationUser, string>, IUserEmailStore<ApplicationUser>, IUserTwoFactorStore<ApplicationUser, string>
    {
        public Task CreateAsync(ApplicationUser user)
        {
            return Task.FromResult(0L);
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            return Task.FromResult(0L);
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            return Task.FromResult(0L);
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            if (userId.Contains("a"))
            {
                return await Task<ApplicationUser>.Factory.StartNew(() => null);
            }
            var task = Task<ApplicationUser>.Factory.StartNew(() => new ApplicationUser
            {
                Id = "uniqueId",
                Email = "mini@mini.cz"
            });
            return await task;
        }

        public async Task<ApplicationUser> FindAsync(string userName, string password)
        {
            if (userName.Contains("a"))
            {
                return await Task<ApplicationUser>.Factory.StartNew(() => null);
            }
            var task = Task<ApplicationUser>.Factory.StartNew(() => new ApplicationUser
            {
                Id = "uniqueId",
                Email = "mini@mini.cz"
            });
            return await task;
        }

        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            if (userName.Contains("a"))
            {
                return await Task<ApplicationUser>.Factory.StartNew(() => null);
            }
            var task = Task<ApplicationUser>.Factory.StartNew(() => new ApplicationUser
            {
                Id = "uniqueId",
                Email = "mini@mini.cz"
            });
            return await task;
        }

        public void Dispose()
        {
            //TODO dispose
        }

        public async Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            var task = Task.Factory.StartNew(() => user.PasswordHash = passwordHash);
            await task;
        }

        public async Task<string> GetPasswordHashAsync(ApplicationUser user)
        {

            var task = Task<string>.Factory.StartNew(() => user.PasswordHash);
            return await task;
        }

        public async Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            var task = Task<bool>.Factory.StartNew(() => !user.PasswordHash.IsEmpty());
            return await task;
        }

        public async Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        {
            var task = Task<DateTimeOffset>.Factory.StartNew(() => new DateTimeOffset());
            return await task;
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            return Task.FromResult(0L);
        }

        public async Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            var task = Task<int>.Factory.StartNew(() => 1);
            return await task;
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            return Task.FromResult(0L);
        }

        public async Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            var task = Task<int>.Factory.StartNew(() => 0);
            return await task;
        }

        public async Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
        {
            var task = Task<bool>.Factory.StartNew(() => false);
            return await task;
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            return Task.FromResult(0L);
        }

        public async Task SetEmailAsync(ApplicationUser user, string email)
        {
            var task = Task.Factory.StartNew(() => user.Email = email);
            await task;
        }

        public async Task<string> GetEmailAsync(ApplicationUser user)
        {
            var task = Task<string>.Factory.StartNew(() => user.Email);
            return await task;
        }

        public async Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            var task = Task<bool>.Factory.StartNew(() => true); //TODO
            return await task;
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            return Task.FromResult(0L);
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            if (email.Contains("a"))
            {
                return await Task<ApplicationUser>.Factory.StartNew(() => null);
            }
            var task = Task<ApplicationUser>.Factory.StartNew(() => new ApplicationUser //TODO
            {
                Id = "uniqueId",
                Email = "mini@mini.cz"
            });
            return await task;
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            return Task.FromResult(0L);
        }

        public async Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
        {
            var task = Task<bool>.Factory.StartNew(() => false);
            return await task;
        }
    }
}
