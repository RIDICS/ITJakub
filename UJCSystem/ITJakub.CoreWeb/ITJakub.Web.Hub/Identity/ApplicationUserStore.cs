using System;
using System.Threading;
using System.Threading.Tasks;
using ITJakub.Shared.Contracts;
using Microsoft.AspNetCore.Identity;

namespace ITJakub.Web.Hub.Identity
{
    public class ApplicationUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserLockoutStore<ApplicationUser>,
        IUserEmailStore<ApplicationUser>, IUserTwoFactorStore<ApplicationUser>
    {
        private readonly CommunicationProvider m_communication;

        public ApplicationUserStore(CommunicationProvider communicationProvider)
        {
            m_communication = communicationProvider;
        }

        public async Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            var task = Task.Factory.StartNew(() => user.Email = email);
            await task;
        }
        
        public async Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var task = Task<string>.Factory.StartNew(() => user.Email);
            return await task;
        }

        public async Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var task = Task<bool>.Factory.StartNew(() => true);
            return await task;
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<ApplicationUser> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email.ToUpperInvariant());
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public async Task<int> GetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var task = Task<int>.Factory.StartNew(() => 0);
            return await task;
        }

        public async Task<bool> GetLockoutEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var task = Task<bool>.Factory.StartNew(() => false);
            return await task;
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public async Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            var task = Task.Factory.StartNew(() => user.PasswordHash = passwordHash);
            await task;
        }

        public async Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var task = Task<string>.Factory.StartNew(() => user.PasswordHash);
            return await task;
        }

        public async Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var task = Task<bool>.Factory.StartNew(() => !string.IsNullOrEmpty(user.PasswordHash));
            return await task;
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName.ToUpperInvariant());
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
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
                    return IdentityResult.Success;
                }

            });
            return await task;
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
        
        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
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

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var task = Task<ApplicationUser>.Factory.StartNew(() =>
            {
                using (var client = m_communication.GetEncryptedClient())
                {
                    var user = client.FindUserByUserName(normalizedUserName);
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

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public async Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var task = Task<bool>.Factory.StartNew(() => false);
            return await task;
        }
    }
}