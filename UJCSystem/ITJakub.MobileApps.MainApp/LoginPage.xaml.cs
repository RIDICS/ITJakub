using System.Threading.Tasks;
using ITJakub.MobileApps.MainApp.Common;
using ITJakub.MobileApps.MainApp.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231
using Microsoft.Live;

namespace ITJakub.MobileApps.MainApp
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class GroupedItemsPage : Page
    {
        private NavigationHelper navigationHelper;

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public GroupedItemsPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
        }


        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            await updateUserName(pageTitle, false);
        }

        public static async Task updateUserName(TextBlock userName, Boolean signIn)
        {
            try
            {
                // Open Live Connect SDK client.
                LiveAuthClient LCAuth = new LiveAuthClient();
                LiveLoginResult LCLoginResult = await LCAuth.InitializeAsync();
                try
                {
                    LiveLoginResult loginResult = null;
                    if (signIn)
                    {
                        // Sign in to the user's Microsoft account with the required scope.
                        //  
                        //  This call will display the Microsoft account sign-in screen if 
                        //   the user is not already signed in to their Microsoft account 
                        //   through Windows 8.
                        // 
                        //  This call will also display the consent dialog, if the user has 
                        //   has not already given consent to this app to access the data 
                        //   described by the scope.
                        // 
                        //  Change the parameter of LoginAsync to include the scopes 
                        //   required by your app.
                        loginResult = await LCAuth.LoginAsync(new string[] { "wl.basic" });
                    }
                    else
                    {
                        // If we don't want the user to sign in, continue with the current 
                        //  sign-in state.
                        loginResult = LCLoginResult;
                    }
                    if (loginResult.Status == LiveConnectSessionStatus.Connected)
                    {
                        // Create a client session to get the profile data.
                        LiveConnectClient connect = new LiveConnectClient(LCAuth.Session);

                        // Get the profile info of the user.
                        LiveOperationResult operationResult = await connect.GetAsync("me");
                        dynamic result = operationResult.Result;
                        if (result != null)
                        {
                            // Update the text of the object passed in to the method. 
                            userName.Text = string.Join(" ", "Hello", result.name, "!");
                        }
                        else
                        {
                            // Handle the case where the user name was not returned. 
                        }
                    }
                    else
                    {
                        // The user hasn't signed in so display this text 
                        //  in place of his or her name.
                        userName.Text = "You're not signed in.";
                    }
                }
                catch (LiveAuthException exception)
                {
                    // Handle the exception. 
                }
            }
            catch (LiveAuthException exception)
            {
                // Handle the exception. 
            }
            catch (LiveConnectException exception)
            {
                // Handle the exception. 
            }
        }
    }
}