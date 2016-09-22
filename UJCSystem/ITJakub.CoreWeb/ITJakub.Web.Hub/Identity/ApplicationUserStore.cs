using System;
using System.Threading.Tasks;
using ITJakub.Shared.Contracts;
using Microsoft.AspNet.Identity;

namespace ITJakub.Web.Hub.Identity
{
    public class ApplicationUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserLockoutStore<ApplicationUser, string>,
        IUserEmailStore<ApplicationUser>, IUserTwoFactorStore<ApplicationUser, string>
    {
        private readonly CommunicationProvider m_communication = new CommunicationProvider();        

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
            var task = Task<bool>.Factory.StartNew(() => true);
            return await task;
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            throw new NotSupportedException();
        }

        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            throw new NotSupportedException();
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        {
            throw new NotSupportedException();
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            throw new NotSupportedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            throw new NotSupportedException();
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            throw new NotSupportedException();
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
            throw new NotSupportedException();
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
            var task = Task<bool>.Factory.StartNew(() => !string.IsNullOrEmpty(user.PasswordHash));
            return await task;
        }

        public async Task CreateAsync(ApplicationUser user)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var userContract = new UserContract
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreateTime = user.CreateTime,
                    PasswordHash = user.PasswordHash,
                    CommunicationToken = user.CommunicationToken
                };

                using (var client = m_communication.GetEncryptedClient())
                {
                    var result = client.CreateUser(userContract);
                    user.CreateTime = result.CreateTime;
                    user.CommunicationToken = result.CommunicationToken;
                    user.Id = result.Id.ToString();
                }

            });
            await task;
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            throw new NotSupportedException();
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            throw new NotSupportedException();
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            var task = Task<ApplicationUser>.Factory.StartNew(() =>
            {
                using (var client = m_communication.GetEncryptedClient())
                {
                    var user = client.FindUserById(int.Parse(userId));
                    if (user == null) return null;                    

                    return new ApplicationUser
                    {
                        Id = user.Id.ToString(),
                        UserName = user.UserName,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CreateTime = user.CreateTime,
                        PasswordHash = user.PasswordHash,
                        CommunicationToken = user.CommunicationToken
                    };
                }
            });

            return await task;
        }

        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var task = Task<ApplicationUser>.Factory.StartNew(() =>
            {
                using (var client = m_communication.GetEncryptedClient())
                {
                    var user = client.FindUserByUserName(userName);
                    if (user == null) return null;

                    return new ApplicationUser
                    {
                        Id = user.Id.ToString(),
                        UserName = user.UserName,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CreateTime = user.CreateTime,
                        PasswordHash = user.PasswordHash,
                        CommunicationToken = user.CommunicationToken,
                        CommunicationTokenExpirationTime = user.CommunicationTokenExpirationTime
                    };
                }
            });
            return await task;
        }

        public void Dispose()
        {
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            throw new NotSupportedException();
        }

        public async Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
        {
            var task = Task<bool>.Factory.StartNew(() => false);
            return await task;
        }

        public async Task<ApplicationUser> FindAsync(string userName, string password)
        {
            return await FindByNameAsync(userName);
        }

    }
}