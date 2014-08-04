using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Facebook;
using ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationBroker;
using ITJakub.MobileApps.Client.Core.ViewModel;

namespace ITJakub.MobileApps.Client.Core.Manager.Authentication
{
    public class FacebookProvider : ILoginProvider
    {
        private const long ClientId = ***REMOVED***;
        //Standard redirect uri for desktop/non-web based apps
        private const string RedirectUri = "https://www.facebook.com/connect/login_success.html";

        public async Task<UserInfo> LoginAsync()
        {
            var fbClient = new FacebookClient();
            //var redirectUri = new Uri(WebAuthenticationBroker.GetCurrentApplicationCallbackUri().AbsoluteUri);
            var redirectUri = new Uri(RedirectUri);
            Uri loginUrl = fbClient.GetLoginUrl(new
            {
                client_id = ClientId,
                redirect_uri = redirectUri,
                scope = "email",
                display = "popup",
                response_type = "token"
            });

            //WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, loginUrl);
            FlexibleWebAuthenticationResult webAuthenticationResult =
                await FlexibleWebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, loginUrl, redirectUri);
            UserInfo userInfo = GetUserInfoFromResponse(fbClient, webAuthenticationResult);

            return userInfo;
        }

        public string AccountName
        {
            get { return "Facebook"; }
        }

        public LoginProviderType ProviderType
        {
            get { return LoginProviderType.Facebook; }
        }

        private UserInfo GetUserInfoFromResponse(FacebookClient fbClient, WebAuthenticationResult webAuthenticationResult)
        {
            var userInfo = new UserInfo();

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

        private UserInfo GetUserInfoFromResponse(FacebookClient fbClient, FlexibleWebAuthenticationResult webAuthenticationResult)
        {
            var userInfo = new UserInfo();

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

        private void FillUserDetailInfo(UserInfo userInfo, string accessToken)
        {
            var fbClient = new FacebookClient(accessToken);
            object user = fbClient.GetTaskAsync("/me").Result;
            var result = (IDictionary<string, object>) user;

            userInfo.Email = result["email"].ToString();
            userInfo.FirstName = result["first_name"].ToString();
            userInfo.LastName = result["last_name"].ToString();
        }
    }
}