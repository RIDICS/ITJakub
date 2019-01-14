﻿using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using Vokabular.Shared.Const;

namespace Vokabular.Shared.AspNetCore.Extensions
{
    public static class UserClaimsExtensions
    {
        public static int GetId(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value);
            }

            throw new AuthenticationException("User is not authenticated");
        }

        public static string GetUserName(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return claimsPrincipal.FindFirst(ClaimTypes.Name).Value;
            }

            throw new AuthenticationException("User is not authenticated");
        }

        public static string GetFirstName(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return claimsPrincipal.FindFirst(ClaimTypes.GivenName).Value;
            }

            throw new AuthenticationException("User is not authenticated");
        }

        public static string GetLastName(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return claimsPrincipal.FindFirst(ClaimTypes.Surname).Value;
            }

            throw new AuthenticationException("User is not authenticated");
        }

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return claimsPrincipal.FindFirst(ClaimTypes.Email).Value;
            }

            throw new AuthenticationException("User is not authenticated");
        }

        public static bool HasPermission(this ClaimsPrincipal claimsPrincipal, string permissionName)
        {
            return claimsPrincipal.Claims.Where(x => x.Type == CustomClaimTypes.Permission)
                .Any(y => y.Value == permissionName);
        }
    }
}
