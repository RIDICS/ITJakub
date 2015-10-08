using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.WebPages;
using ITJakub.ITJakubService.DataContracts.Clients;
using ITJakub.Shared.Contracts;
using Microsoft.AspNet.Identity;

namespace ITJakub.Web.Hub.Identity
{
    public class ApplicationUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserLockoutStore<ApplicationUser, string>,
        IUserEmailStore<ApplicationUser>, IUserTwoFactorStore<ApplicationUser, string>, IUserRoleStore<ApplicationUser>, IUserClaimStore<ApplicationUser>
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
            var task = Task<bool>.Factory.StartNew(() => !user.PasswordHash.IsEmpty());
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

                using (var client = m_communication.GetAuthenticatedClient(user.UserName, user.CommunicationToken))
                {
                    var specialPermissions = client.GetSpecialPermissionsForUserByType(SpecialPermissionCategorizationEnumContract.Action);
                    user.SpecialPermissions = specialPermissions;
                }

                var claims = GetClaimsFromSpecialPermissions(user.SpecialPermissions);
                claims.Add(new Claim(CustomClaimType.CommunicationToken, user.CommunicationToken));
                user.Claims = claims;

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

                    IList<SpecialPermissionContract> specialPermissions;
                    using (var authenticatedClient = m_communication.GetAuthenticatedClient(user.UserName, user.CommunicationToken))
                    {
                        specialPermissions = authenticatedClient.GetSpecialPermissionsForUserByType(SpecialPermissionCategorizationEnumContract.Action);
                    }

                    var claims = GetClaimsFromSpecialPermissions(specialPermissions);
                    claims.Add(new Claim(CustomClaimType.CommunicationToken, user.CommunicationToken));

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
                        SpecialPermissions = specialPermissions,
                        Claims = claims
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

                    IList<SpecialPermissionContract> specialPermissions;
                    using (var authenticatedClient = m_communication.GetAuthenticatedClient(user.UserName, user.CommunicationToken))
                    {
                        specialPermissions = authenticatedClient.GetSpecialPermissionsForUserByType(SpecialPermissionCategorizationEnumContract.Action);
                    }

                    var claims = GetClaimsFromSpecialPermissions(specialPermissions);                   
                    claims.Add(new Claim(CustomClaimType.CommunicationToken, user.CommunicationToken));

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
                        SpecialPermissions = specialPermissions,
                        Claims = claims
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

        public async Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            var claims = await GetClaimsAsync(user);
            var roleClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role && x.Value == roleName);
            if (roleClaim == null)
            {
                roleClaim = new Claim(ClaimTypes.Role, roleName);
                var task = Task.Factory.StartNew(() => user.Claims.Add(roleClaim));
                await task;
            }
        }

        public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            var claims = await GetClaimsAsync(user);
            var roleClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role && x.Value == roleName);
            if (roleClaim != null)
            {
                var task = Task.Factory.StartNew(() => RemoveClaimAsync(user, roleClaim));
                await task;
            }
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            var task = Task<IList<string>>.Factory.StartNew(() =>
            {
                var roles = user.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
                return roles;
            });

            return await task;
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            var task = Task<bool>.Factory.StartNew(() =>
            {
                return user.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value == roleName);
            });

            return await task;
        }

        public async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var task = Task<IList<Claim>>.Factory.StartNew(() => user.Claims);
            return await task;
        }

        public async Task AddClaimAsync(ApplicationUser user, Claim claim)
        {
            var task = Task.Factory.StartNew(() => user.Claims.Add(claim));
            await task;
        }

        public async Task RemoveClaimAsync(ApplicationUser user, Claim claim)
        {
            var task = Task.Factory.StartNew(() => user.Claims.Remove(claim));
            await task;
        }


        private IList<Claim> GetClaimsFromSpecialPermissions(IList<SpecialPermissionContract> specialPermissions)
        {
            var claims = new List<Claim>();

            if (specialPermissions.Count != 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, CustomRole.CanViewAdminModule));
            }

            if (specialPermissions.OfType<UploadBookPermissionContract>().Count() != 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, CustomRole.CanUploadBooks));
            }

            if (specialPermissions.OfType<NewsPermissionContract>().Count() != 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, CustomRole.CanAddNews));
            }

            if (specialPermissions.OfType<ManagePermissionsPermissionContract>().Count() != 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, CustomRole.CanManagePermissions));
            }

            if (specialPermissions.OfType<FeedbackPermissionContract>().Count() != 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, CustomRole.CanManageFeedbacks));
            }

            return claims;
        }
    }
}