using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Facebook;
using ITJakub.MobileApps.Client.Core.ViewModel;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication
{
    public class FacebookManager : IAuthProvider
    {
        private const long ClientId = ***REMOVED***;

        public override async Task<UserInfo> LoginAsync()
        {
            var fbClient = new FacebookClient();
            var loginUrl = fbClient.GetLoginUrl(new
            {
                client_id = ClientId,
                redirect_uri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().AbsoluteUri,
                scope = "email",
                display = "popup",
                response_type = "token"
            });

            var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, loginUrl);
            var userInfo = GetUserInfoFromResponse(fbClient, webAuthenticationResult);

            return userInfo;
        }

        private UserInfo GetUserInfoFromResponse(FacebookClient fbClient, WebAuthenticationResult webAuthenticationResult)
        {
            var userInfo = new UserInfo();

            switch (webAuthenticationResult.ResponseStatus)
            {
                case WebAuthenticationStatus.Success:
                    var callbackUri = new Uri(webAuthenticationResult.ResponseData);
                    var facebookOAuthResult = fbClient.ParseOAuthCallbackUrl(callbackUri);

                    // Retrieve the Access Token. You can now interact with Facebook on behalf of the user
                    // using the Access Token.
                    var accessToken = facebookOAuthResult.AccessToken;

                    userInfo.Success = true;
                    userInfo.AccessToken = accessToken;
                    FillUserDetailInfo(userInfo, accessToken);
                    break;

                case WebAuthenticationStatus.ErrorHttp:
                    // handle authentication failure
                    userInfo.Success = false;
                    break;

                default:
                    // The user canceled the authentication
                    userInfo.Success = false;
                    break;
            }
            return userInfo;
        }

        private void FillUserDetailInfo(UserInfo userInfo, string accessToken)
        {
            var fbClient = new FacebookClient(accessToken);
            var user = fbClient.GetTaskAsync("/me").Result;
            var result = (IDictionary<string, object>)user;

            userInfo.Email = result["email"].ToString();
            userInfo.FirstName = result["first_name"].ToString();
            userInfo.LastName = result["last_name"].ToString();
        }
    }
}