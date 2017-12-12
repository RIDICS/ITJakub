using Microsoft.AspNetCore.Identity;

namespace ITJakub.Web.Hub.Core.Identity
{
    public class CustomPasswordHasher: IPasswordHasher<ApplicationUser>
    {
        public string HashPassword(ApplicationUser user, string password)
        {
            return Vokabular.Jewelry.CustomPasswordHasher.CreateHash(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
        {
            var result = Vokabular.Jewelry.CustomPasswordHasher.ValidatePassword(providedPassword, hashedPassword);
            if(result)
                return PasswordVerificationResult.Success;

            return PasswordVerificationResult.Failed;
        }
    }
}