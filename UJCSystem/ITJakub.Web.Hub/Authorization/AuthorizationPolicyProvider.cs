using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Vokabular.Shared.Const;

namespace ITJakub.Web.Hub.Authorization
{
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private const string PermissionName = CustomClaimTypes.Permission;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationPolicyProvider"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        /// <summary>
        /// Gets a <see cref="T:Microsoft.AspNetCore.Authorization.AuthorizationPolicy" /> from the given <paramref name="policyName" />
        /// </summary>
        /// <param name="policyName">The policy name to retrieve.</param>
        /// <returns>
        /// The named <see cref="T:Microsoft.AspNetCore.Authorization.AuthorizationPolicy" />.
        /// </returns>
        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            // check static policies first
            var policy = await base.GetPolicyAsync(policyName);

            if (policy != null) return policy;

            var splitPermissions = policyName.Split(",");
            policy = new AuthorizationPolicyBuilder()
                .RequireClaim(PermissionName, splitPermissions)
                .Build();

            return policy;
        }
    }
}
