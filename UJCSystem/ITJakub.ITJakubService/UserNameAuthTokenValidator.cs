using System;
using System.IdentityModel.Tokens;
using ITJakub.ITJakubService.Core;

namespace ITJakub.ITJakubService
{
    public class UserNameAuthTokenValidator : System.IdentityModel.Selectors.UserNamePasswordValidator
    {
        private readonly AuthenticationManager m_authorizationManager = Container.Current.Resolve<AuthenticationManager>();

        private static string communicationTokenPrefix = "CT:";
        private static string passwordPrefix = "PW:";


        public override void Validate(string userName, string authenticationToken)
        {

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(authenticationToken))
            {
                throw new ArgumentNullException();
            }
            
            if (authenticationToken.StartsWith(communicationTokenPrefix))
            {
                m_authorizationManager.ValidateUserAndCommToken(userName, authenticationToken);
                return;
            }

            if (authenticationToken.StartsWith(passwordPrefix))
            {
                m_authorizationManager.ValidateUserAndPassword(userName, authenticationToken);
                return;
            }


            //Authenticate user by username and token
            throw new ArgumentException("Invalid auth token format");
            

        }
    }
}