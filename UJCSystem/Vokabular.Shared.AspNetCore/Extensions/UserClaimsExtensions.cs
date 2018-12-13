using System;
using System.Security.Claims;

namespace Vokabular.Shared.AspNetCore.Extensions
{
    public static class UserClaimsExtensions
    {
        public static long GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return long.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value);
            }

            throw new ArgumentException("User is not authenticated");
        }

        public static string GetUserName(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return claimsPrincipal.FindFirst(ClaimTypes.Name).Value;
            }

            throw new ArgumentException("User is not authenticated");
        }

        public static string GetFirstName(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return claimsPrincipal.FindFirst(ClaimTypes.GivenName).Value;
            }

            throw new ArgumentException("User is not authenticated");
        }

        public static string GetLastName(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return claimsPrincipal.FindFirst(ClaimTypes.Surname).Value;
            }

            throw new ArgumentException("User is not authenticated");
        }

        public static string GetUserEmail(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return claimsPrincipal.FindFirst(ClaimTypes.Email).Value;
            }

            throw new ArgumentException("User is not authenticated");
        }
    }
}
