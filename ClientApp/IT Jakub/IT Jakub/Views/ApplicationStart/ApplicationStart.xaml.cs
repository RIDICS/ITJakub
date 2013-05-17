using IT_Jakub.Classes.Autentifcation;
using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Exceptions;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Utils;
using IT_Jakub.Views.UserManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IT_Jakub.Views.ApplicationStart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ApplicationStart : Page
    {

        private static LoggedUser lu = LoggedUser.getInstance();
        private static bool isCredentialsCorrect = true;

        public ApplicationStart()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            if (lu.isLoggedIn()) {
                this.Frame.Navigate(typeof(UserLoggedIn.SessionsList));
            } else {
                CredentialPickerResults credentials = await MyDialogs.showLoginDialog(isCredentialsCorrect);

                UserTable us = new UserTable();
                try {
                    User u = await us.getUserByUsername(credentials.CredentialUserName.Trim());
                    Authenticator a = new Authenticator();
                    isCredentialsCorrect = a.authenticateUser(u, credentials.CredentialPassword);
                    if (isCredentialsCorrect) {
                        lu.login(u);
                        this.Frame.Navigate(typeof(UserLoggedIn.SessionsList));
                    } else {
                        // MyDialogs.showDialogOK("Nesprávné uživatelské jméno nebo heslo");
                        this.Frame.Navigate(typeof(ApplicationStart));
                    }
                } catch (ServerErrorException ex) {
                    throw new ServerErrorException(ex);
                } catch (UserNotFoundException ex) {
                    throw new UserNotFoundException(ex);
                }
            }
        }

        private void userManagement_Click(object sender, RoutedEventArgs e) {
            if (this.Frame != null) {
                this.Frame.Navigate(typeof(UserManagementStart));
            }
        }
    }
}
