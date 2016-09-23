using Microsoft.AspNetCore.Identity;

namespace ITJakub.Web.Hub.Identity
{
    public class CustomPasswordHasher: IPasswordHasher<ApplicationUser>
    {
        public string HashPassword(ApplicationUser user, string password)
        {
            return Jewelery.CustomPasswordHasher.CreateHash(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
        {
           var result =  Jewelery.CustomPasswordHasher.ValidatePassword(providedPassword, hashedPassword);
            if(result)
                return PasswordVerificationResult.Success;

            return PasswordVerificationResult.Failed;
        }
    }
}