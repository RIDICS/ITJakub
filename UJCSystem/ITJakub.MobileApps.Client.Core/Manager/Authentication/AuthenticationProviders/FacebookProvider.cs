using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Facebook;
using ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationBroker;
using ITJakub.MobileApps.Client.Core.ViewModel;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationProviders
{
    public class FacebookProvider : ILoginProvider
    {
        private const long ClientId = ***REMOVED***;
        //Standard redirect uri for desktop/non-web based apps
        private const string RedirectUri = "https://www.facebook.com/connect/login_success.html";

        public async Task<UserLoginSkeleton> LoginAsync()
        {
            var fbClient = new FacebookClient();
            var redirectUri = new Uri(RedirectUri);
            Uri loginUrl = fbClient.GetLoginUrl(new
            {
                client_id = ClientId,
                redirect_uri = redirectUri,
                scope = "email",
                display = "popup",
                response_type = "token"
            });
            
            AuthBrokerResult webAuthenticationResult =
                await CustomWebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, loginUrl, redirectUri);
            UserLoginSkeleton userLoginSkeleton = GetUserInfoFromResponse(fbClient, webAuthenticationResult);

            return userLoginSkeleton;
        }

        public string AccountName
        {
            get { return "Facebook"; }
        }

        public LoginProviderType ProviderType
        {
            get { return LoginProviderType.Facebook; }
        }

        private UserLoginSkeleton GetUserInfoFromResponse(FacebookClient fbClient, WebAuthenticationResult webAuthenticationResult)
        {
            var userInfo = new UserLoginSkeleton();

            switch (webAuthenticationResult.ResponseStatus)
            {
                case WebAuthenticationStatus.Success:
                    var callbackUri = new Uri(webAuthenticationResult.ResponseData);
                    FacebookOAuthResult facebookOAuthResult = fbClient.ParseOAuthCallbackUrl(callbackUri);

                    // Retrieve the Access Token. You can now interact with Facebook on behalf of the user
                    // using the Access Token.
                    string accessToken = facebookOAuthResult.AccessToken;

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

        private UserLoginSkeleton GetUserInfoFromResponse(FacebookClient fbClient, AuthBrokerResult webAuthenticationResult)
        {
            var userInfo = new UserLoginSkeleton();

            switch (webAuthenticationResult.ResponseStatus)
            {
                case WebAuthenticationStatus.Success:
                    var callbackUri = new Uri(webAuthenticationResult.ResponseData);
                    FacebookOAuthResult facebookOAuthResult = fbClient.ParseOAuthCallbackUrl(callbackUri);

                    // Retrieve the Access Token. You can now interact with Facebook on behalf of the user
                    // using the Access Token.
                    string accessToken = facebookOAuthResult.AccessToken;

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

        private void FillUserDetailInfo(UserLoginSkeleton userLoginSkeleton, string accessToken)
        {
            var fbClient = new FacebookClient(accessToken);
            object user = fbClient.GetTaskAsync("/me").Result;
            var result = (IDictionary<string, object>) user;

            userLoginSkeleton.Email = result["email"].ToString();
            userLoginSkeleton.FirstName = result["first_name"].ToString();
            userLoginSkeleton.LastName = result["last_name"].ToString();
        }
    }
}