using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Autentifcation;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Networking;
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
using Windows.UI;
using Windows.UI.Popups;
using IT_Jakub.Classes.Exceptions;
using IT_Jakub.Classes.Utils;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IT_Jakub.Views.ApplicationStart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        
        private bool isUsername_FirstFocus = true;
        private bool isPassword_FirstFocus = true;
        private static LoggedUser lu = LoggedUser.getInstance();

        public LoginPage() {
            this.InitializeComponent();
        }



        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }
        
        private void clearUsernameText() {
            username.Text = "";
        }

        private void clearPasswordPassword() {
            password.Password = "";
        }

        private async void loginButtonClicked(object sender, RoutedEventArgs e) {
            bool isCredentialsCorrect = false;
            UserTable us = new UserTable();
            try {
                User u = await us.getUserByUsername(username.Text.Trim());
                Authenticator a = new Authenticator();
                isCredentialsCorrect = a.authenticateUser(u, password.Password);
                if (isCredentialsCorrect) {
                    lu.login(u);
                    this.Frame.Navigate(typeof(UserLoggedIn.UserLoggedIn));
                } else {
                    MyDialogs.showDialogOK("Nesprávné uživatelské jméno nebo heslo");
                    errorTextBlock.Visibility = Visibility.Visible;
                    errorTextBlock.Text = "Nesprávné uživatelské jméno nebo heslo";
                    Color redColor = Color.FromArgb(255, 255, 0, 0);
                    password.BorderBrush = new SolidColorBrush(redColor);
                    username.BorderBrush = new SolidColorBrush(redColor);
                }
            } catch (ServerErrorException ex) {
                throw new ServerErrorException(ex);
            } catch (UserNotFoundException ex) {
                throw new UserNotFoundException(ex);
            }
        }

        private void username_GetFocus(object sender, RoutedEventArgs e) {
            if (isUsername_FirstFocus) {
                clearUsernameText();
                isUsername_FirstFocus = false;
            }
        }

        private void password_GetFocus(object sender, RoutedEventArgs e) {
            if (isPassword_FirstFocus) {
                clearPasswordPassword();
                isPassword_FirstFocus = false;
            }
        }
        
    }
}
