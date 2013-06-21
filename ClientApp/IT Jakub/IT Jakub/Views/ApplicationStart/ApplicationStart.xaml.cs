using IT_Jakub.Classes.Authentication;
using IT_Jakub.Classes.DatabaseModels;
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

namespace IT_Jakub.Views.ApplicationStart {
    /// <summary>
    /// Page shown at application start.
    /// </summary>
    public sealed partial class ApplicationStart : Page {

        /// <summary>
        /// The lu is singleton instance of LoggedUser. LoggedUser is user which is currently logged in.
        /// </summary>
        private static LoggedUser lu = LoggedUser.getInstance();

        /// <summary>
        /// Means if loggin credentials are correct
        /// </summary>
        private static bool isCredentialsCorrect = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationStart"/> class.
        /// </summary>
        public ApplicationStart() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
        }

        private void userManagement_Click(object sender, RoutedEventArgs e) {
            if (this.Frame != null) {
                this.Frame.Navigate(typeof(UserManagementStart));
            }
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e) {
            loginButton.IsEnabled = false;
            CredentialPickerResults credentials = await MyDialogs.showLoginDialog(isCredentialsCorrect);
            if (!lu.isLoggedIn()) {
                

                UserTable us = new UserTable();
                try {
                    User u = await us.getUserByUsername(credentials.CredentialUserName.Trim());
                    Authenticator a = new Authenticator();
                    isCredentialsCorrect = a.authenticateUser(u, credentials.CredentialPassword);
                    if (isCredentialsCorrect) {
                        lu.login(u);
                        this.Frame.Navigate(typeof(UserLoggedIn.SessionsList));
                    } else {
                        this.Frame.Navigate(typeof(ApplicationStart));
                    }
                } catch (Exception ex) {
                    object o = e;
                    return;
                }
            }
        }

        private void aboutButton_Click(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(About));
        }

        private void helpButton_Click(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(Help));
        }
    }
}
