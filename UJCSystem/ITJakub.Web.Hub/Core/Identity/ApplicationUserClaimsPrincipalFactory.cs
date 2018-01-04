using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ITJakub.Shared.Contracts;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Vokabular.Shared.Const;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Core.Identity
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, CustomRole>
    {
        private readonly CommunicationProvider m_communication;

        public ApplicationUserClaimsPrincipalFactory(CommunicationProvider communicationProvider, UserManager<ApplicationUser> userManager, RoleManager<CustomRole> roleManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
        {
            m_communication = communicationProvider;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);

            var claimsIdentity = (ClaimsIdentity) principal.Identity;
            claimsIdentity.AddClaim(new Claim(CustomClaimType.CommunicationToken, user.CommunicationToken));
                
            using (var authenticatedClient = m_communication.GetAuthenticatedClient(user.UserName, user.CommunicationToken))
            {
                var specialPermissions = authenticatedClient.GetSpecialPermissionsForUserByType(SpecialPermissionCategorizationEnumContract.Action);
                var roleClaims = GetClaimsFromSpecialPermissions(specialPermissions);
                claimsIdentity.AddClaims(roleClaims);
            }

            return principal;
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

            if (specialPermissions.OfType<ReadLemmatizationPermissionContract>().Count() != 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, CustomRole.CanReadLemmatization));
            }

            if (specialPermissions.OfType<EditLemmatizationPermissionContract>().Count() != 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, CustomRole.CanEditLemmatization));
            }

            if (specialPermissions.OfType<DerivateLemmatizationPermissionContract>().Count() != 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, CustomRole.CanDerivateLemmatization));
            }

            if (specialPermissions.OfType<EditionPrintPermissionContract>().Count() != 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, CustomRole.CanEditionPrint));
            }

            if (specialPermissions.OfType<EditStaticTextPermissionContract>().Count() != 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, CustomRole.CanEditStaticText));
            }

            return claims;
        }
    }
}