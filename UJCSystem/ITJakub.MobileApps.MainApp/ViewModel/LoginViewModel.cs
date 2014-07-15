using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Security.Authentication.Web;
using Windows.Storage.Streams;
using Facebook;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

namespace ITJakub.MobileApps.MainApp.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        private RelayCommand m_facebookLoginCommand;
        private RelayCommand m_googleLoginCommand;

        /// <summary>
        /// Initializes a new instance of the LoginViewModel class.
        /// </summary>
        public LoginViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            
            m_facebookLoginCommand = new RelayCommand(FacebookLogin);
            m_googleLoginCommand = new RelayCommand(GoogleLogin);
        }

        public RelayCommand FacebookLoginCommand
        {
            get { return m_facebookLoginCommand; }
        }

        public string Message { get; set; }

        public RelayCommand GoogleLoginCommand
        {
            get { return m_googleLoginCommand; }
        }

        private async void FacebookLogin()
        {
            var fbClient = new FacebookClient();

            var loginUrl = fbClient.GetLoginUrl(new
            {
                client_id = ***REMOVED***,
                redirect_uri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().AbsoluteUri,
                scope = "email",
                display = "popup",
                response_type = "token"
            });

            var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                  WebAuthenticationOptions.None,
                  loginUrl);

            switch (webAuthenticationResult.ResponseStatus)
            {
                case WebAuthenticationStatus.Success:
                    var callbackUri = new Uri(webAuthenticationResult.ResponseData.ToString());
                    var facebookOAuthResult = fbClient.ParseOAuthCallbackUrl(callbackUri);

                    // Retrieve the Access Token. You can now interact with Facebook on behalf of the user
                    // using the Access Token.
                    var accessToken = facebookOAuthResult.AccessToken;

                    fbClient = new FacebookClient(accessToken);
                    var user = await fbClient.GetTaskAsync("/me");
                    var result = (IDictionary<string, object>)user;

                    Message = String.Format("First name: {0}, Last name: {1}, Name: {2}, E-mail: {3}, Access token: {4}", result["first_name"], result["last_name"], result["name"], result["email"], accessToken);
                    RaisePropertyChanged(() => Message);
                    break;
                case WebAuthenticationStatus.ErrorHttp:
                    // handle authentication failure
                    break;
                default:
                    // The user canceled the authentication
                    break;
            }
        }

        private async void GoogleLogin()
        {
             var startURL =
                 "https://accounts.google.com/o/oauth2/auth?client_id=***REMOVED***&response_type=code&scope=openid%20email&redirect_uri=urn:ietf:wg:oauth:2.0:oob";
            

             var startURI = new Uri(startURL);
             var endUri = new Uri("https://accounts.google.com/o/oauth2/approval?");
            
             string result;

             try
             {
                 var webAuthenticationResult =
                     await WebAuthenticationBroker.AuthenticateAsync(
                     WebAuthenticationOptions.UseTitle,
                     startURI,
                     endUri
                     );

                 switch (webAuthenticationResult.ResponseStatus)
                 {
                     case WebAuthenticationStatus.Success:
                         // Successful authentication. 
                         result = webAuthenticationResult.ResponseData.ToString();
                         break;
                     case WebAuthenticationStatus.ErrorHttp:
                         // HTTP error. 
                         result = webAuthenticationResult.ResponseErrorDetail.ToString();
                         break;
                     default:
                         // Other error.
                         result = webAuthenticationResult.ResponseData.ToString();
                         break;
                 }
             }
             catch (Exception ex)
             {
                 // Authentication failed. Handle parameter, SSL/TLS, and Network Unavailable errors here. 
                 result = ex.Message;
             }
             Message = result;
             RaisePropertyChanged(() => Message);

             

          
            
               /*  var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                         new Uri("ms-appx:///Assets/client_secrets.json"),
                         new[] { Uri.EscapeUriString(CalendarService.Scope.Calendar) },
                         "user",
                         CancellationToken.None);*/
            
        }
    }
}