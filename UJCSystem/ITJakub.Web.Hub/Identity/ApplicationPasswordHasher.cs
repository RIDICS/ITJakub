using Microsoft.AspNet.Identity;

namespace ITJakub.Web.Hub.Identity
{
    public class ApplicationPasswordHasher : PasswordHasher
    {
        public override string HashPassword(string password)
        {
            return base.HashPassword(password);
        }

        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var hashedPasswd2 = HashPassword(providedPassword); //TODO delete
            return base.VerifyHashedPassword(hashedPasswd2, providedPassword);
            //return base.VerifyHashedPassword(hashedPassword, providedPassword);
        }
    }
}