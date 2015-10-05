using System;
using System.IdentityModel.Tokens;
using ITJakub.ITJakubService.Core;

namespace ITJakub.ITJakubService
{
    public class UserNamePasswordValidator : System.IdentityModel.Selectors.UserNamePasswordValidator
    {

        private readonly AuthenticationManager m_authorizationManager = Container.Current.Resolve<AuthenticationManager>();

        public override void Validate(string userName, string commToken)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(commToken))
            {
                throw new ArgumentNullException();
            }

            if (!m_authorizationManager.ValidateUserAndCommToken(userName, commToken))
                throw new SecurityTokenValidationException("Invalid Credentials");

        }
    }
}