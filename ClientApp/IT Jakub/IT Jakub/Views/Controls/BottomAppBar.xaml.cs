using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace IT_Jakub.Views.Controls {
    public sealed partial class BottomAppBar : UserControl {

        private static LoggedUser lu = LoggedUser.getInstance();
        private static SignedSession ss = SignedSession.getInstance();
        private Frame mainFrame;
        private static Session selectedSession;

        public BottomAppBar() {
            this.InitializeComponent();
        }

        public static void repaint(BottomAppBar bar) {
            if (lu.isLoggedIn()) {
                bar.userIsLoggedInTemplate();
            } else {
                bar.userIsLoggedOutTemplate();
            }

            if (ss.isSignedInSession()) {
                bar.sessionSignedInTemplate();
            } else {
                bar.sessionSignedOutTemplate();
            }
            if (selectedSession != null) {
                bar.signinSessionButton.IsEnabled = true;
            } else {
                bar.signinSessionButton.IsEnabled = false;

            }
        }
        

        private void userIsLoggedInTemplate() {
            logoutButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            loginButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void userIsLoggedOutTemplate() {
            logoutButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            loginButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void sessionSignedInTemplate() {
            signinSessionButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            signoutSessionButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void sessionSignedOutTemplate() {
            signinSessionButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            signoutSessionButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
        
        private void navigateToLoginPage() {
            mainFrame = MainPage.getMainFrame();
            mainFrame.Navigate(typeof(Views.ApplicationStart.LoginPage));
        }

        private async void logoutUser() {
            selectedSession = null;
            await lu.logout();
            mainFrame = MainPage.getMainFrame();
            mainFrame.Navigate(typeof(MainPage));
        }

        private async void signOutSession() {
            selectedSession = null;
            await ss.signout();
            mainFrame = MainPage.getMainFrame();
            mainFrame.Navigate(typeof(Views.UserLoggedIn.SessionsList));
        }

        private void signInSession() {
            if (selectedSession != null) {
                ss.register(selectedSession);
                ss.login();
                selectedSession = null;
            }
            return;
        }
        
        internal static void setSelectedSession(Session s) {
            selectedSession = s;
        }

        private void homeButton_Click(object sender, RoutedEventArgs e) {
            logoutUser();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e) {
            navigateToLoginPage();
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e) {
            logoutUser();
        }

        private void signinSessionButton_Click(object sender, RoutedEventArgs e) {
            signInSession();
        }

        private void signoutSessionButton_Click(object sender, RoutedEventArgs e) {
            signOutSession();
        }
    }
}
