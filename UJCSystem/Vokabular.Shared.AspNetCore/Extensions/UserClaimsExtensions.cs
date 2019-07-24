using System;
using System.Linq;
using System.Security.Claims;
using Vokabular.Shared.Const;

namespace Vokabular.Shared.AspNetCore.Extensions
{
    public static class UserClaimsExtensions
    {
        public static int? GetId(this ClaimsPrincipal claimsPrincipal)
        {
            var idClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);

            if (claimsPrincipal.Identity.IsAuthenticated && idClaim != null)
            {
                return int.Parse(idClaim.Value);
            }

            return null;
        }

        public static string GetUserName(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
            }

            return null;
        }

        public static string GetFirstName(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return claimsPrincipal.FindFirst(ClaimTypes.GivenName)?.Value;
            }

            return null;
        }

        public static string GetLastName(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return claimsPrincipal.FindFirst(ClaimTypes.Surname)?.Value;
            }

            return null;
        }

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
            }

            return null;
        }

        public static bool? IsEmailConfirmed(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return Convert.ToBoolean(claimsPrincipal.FindFirst(CustomClaimTypes.EmailConfirmed)?.Value);
            }

            return null;
        }

        public static bool HasPermission(this ClaimsPrincipal claimsPrincipal, string permissionName)
        {
            return claimsPrincipal.Claims.Where(x => x.Type == CustomClaimTypes.Permission)
                .Any(y => y.Value == permissionName);
        }
    }
}
