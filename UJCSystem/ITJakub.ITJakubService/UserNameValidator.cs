using System;
using System.IdentityModel.Tokens;
using ITJakub.ITJakubService.Core;

namespace ITJakub.ITJakubService
{
    public class UserNamePasswordValidator : System.IdentityModel.Selectors.UserNamePasswordValidator
    {

        private readonly UserManager m_userManager = Container.Current.Resolve<UserManager>();

        public override void Validate(string userName, string commToken)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(commToken))
            {
                throw new ArgumentNullException();
            }

            if (!m_userManager.ValidateUserAndCommToken(userName, commToken))
                throw new SecurityTokenValidationException("Invalid Credentials");

        }
    }
}