using System;
using System.Security.Cryptography;
using Microsoft.AspNet.Identity;

namespace ITJakub.Web.Hub.Identity
{
    public class CustomPasswordHasher: IPasswordHasher
    {        
        public string HashPassword(string password)
        {
            return Jewelery.CustomPasswordHasher.CreateHash(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
           var result =  Jewelery.CustomPasswordHasher.ValidatePassword(providedPassword, hashedPassword);
            if(result)
                return PasswordVerificationResult.Success;

            return PasswordVerificationResult.Failed;
        }
    }
}