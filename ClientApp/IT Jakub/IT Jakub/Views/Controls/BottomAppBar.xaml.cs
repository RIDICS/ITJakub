using Callisto.Controls;
using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Enumerations;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Utils;
using IT_Jakub.Views.Controls.FlyoutControls;
using IT_Jakub.Views.EducationalApplications.SynchronizedReading;
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
using WinRTXamlToolkit.Controls.Extensions;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace IT_Jakub.Views.Controls {
    public sealed partial class BottomAppBar : UserControl {

        private static LoggedUser lu = LoggedUser.getInstance();
        private static SignedSession ss = SignedSession.getInstance();
        private Frame mainFrame;
        private static Session selectedSession;
        private static string currentPage;
        private static UserRights userRights = UserRights.Default;

        public BottomAppBar() {
            this.InitializeComponent();
        }

        public static void repaint(BottomAppBar bar) {
            Frame mainFrame = MainPage.getMainFrame();
            currentPage = mainFrame.CurrentSourcePageType.FullName;

            if (currentPage.Contains("Views.ApplicationStart")) {
                bar.applicationStartTemplate(bar);
            }

            if (currentPage.Contains("Views.UserLoggedIn")) {
                bar.userLoggedInTemplate(bar);
            }

            if (currentPage.Contains("EducationalApplications")) {
                bar.educationalApplicationsTemplate();
            }

            if (ss.isSignedInSession()) {
                bar.sessionSignedInTemplate();
            } else {
                bar.sessionSignedOutTemplate();
            }
        }

        private void educationalApplicationsTemplate() {
            createSession.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            deleteSession.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            findSession.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            signinSessionButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            highlightTextButton.Visibility = Windows.UI.Xaml.Visibility.Visible;

            if (EducationalApplications.SynchronizedReading.SyncReadingApp.isAutoUpdateEnabled()) {
                autoUpdateOnButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                autoUpdateOffButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            } else {
                autoUpdateOnButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                autoUpdateOffButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

            if (currentPage.Contains("SyncReadingApp")) {
                syncReadingAppTemplate();
            }
        }

        private void syncReadingAppTemplate() {
            updateUserlistButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if (userRights == UserRights.Owner) {
                openFileButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                highlightTextButton.IsEnabled = true;
            }
            if (userRights == UserRights.Prefered) {
                openFileButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                highlightTextButton.IsEnabled = true;
            }
            if (userRights == UserRights.Default) {
                openFileButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                highlightTextButton.IsEnabled = false;
            }
        }

        private void applicationStartTemplate(BottomAppBar bar) {
            createSession.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            deleteSession.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            findSession.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            signinSessionButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            signoutSessionButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            logoutButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            updateUserlistButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void userLoggedInTemplate(BottomAppBar bar) {
            updateUserlistButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            createSession.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            deleteSession.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            findSession.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            if (lu.isLoggedIn()) {
                logoutButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            } else {
                logoutButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

            if (currentPage.Contains("Views.UserLoggedIn.SessionsList")) {
                if (lu.getUserData().Role == UserRole.Teacher || lu.getUserData().Role == UserRole.Principal) {
                    createSession.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    deleteSession.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    if (selectedSession != null) {
                        deleteSession.IsEnabled = true;
                    } else {
                        deleteSession.IsEnabled = false;
                    }
                }
                findSession.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }
        

        private void sessionSignedInTemplate() {
            signinSessionButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            signoutSessionButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void sessionSignedOutTemplate() {
            signoutSessionButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            if (currentPage.Contains("Views.UserLoggedIn.SessionsList")) {
                signinSessionButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                if (selectedSession != null) {
                    signinSessionButton.IsEnabled = true;
                } else {
                    signinSessionButton.IsEnabled = false;
                }
            } else {
                signinSessionButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void navigateToLoginPage() {
            mainFrame = MainPage.getMainFrame();
            mainFrame.Navigate(typeof(Views.ApplicationStart.LoginPage));
        }

        private async void logoutUser() {
            selectedSession = null;
            await lu.logout();
            mainFrame = MainPage.getMainFrame();
            mainFrame.Navigate(typeof(ApplicationStart.ApplicationStart));
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

        private void logoutButton_Click(object sender, RoutedEventArgs e) {
            logoutUser();
        }

        private void signinSessionButton_Click(object sender, RoutedEventArgs e) {
            signInSession();
        }

        private void signoutSessionButton_Click(object sender, RoutedEventArgs e) {
            signOutSession();
        }

        private void autoUpdateOnButton_Click(object sender, RoutedEventArgs e) {
            EducationalApplications.SynchronizedReading.SyncReadingApp.setAutoUpdate(true);
            repaint(this);
        }

        private void autoUpdateOffButton_Click(object sender, RoutedEventArgs e) {
            EducationalApplications.SynchronizedReading.SyncReadingApp.setAutoUpdate(false);
            repaint(this);
        }

        private void highlightTextButton_Click(object sender, RoutedEventArgs e) {
            SyncReadingApp.highligtTextButton_Click();
        }

        private void updateUserlistButton_Click(object sender, RoutedEventArgs e) {
            SyncReadingApp.updateUserList();
        }

        public static void setUserRights (UserRights ur) {
            userRights = ur;
        }

        private void openFileButton_Click(object sender, RoutedEventArgs e) {
            Flyout flyOut = new Flyout();
            flyOut.Content = new OpenFileFlyout(flyOut);
            flyOut.PlacementTarget = sender as UIElement;
            flyOut.Placement = PlacementMode.Top;
            flyOut.Width = 400;
            flyOut.IsOpen = true;
        }

        private void deleteSession_Click(object sender, RoutedEventArgs e) {
            if (selectedSession != null) {
                SessionTable st = new SessionTable();
                st.removeSession(selectedSession);

                SessionUserTable sut = new SessionUserTable();
                sut.removeAllUsersFromSession(selectedSession);

                CommandTable ct = new CommandTable();
                ct.removeSessionsCommand(selectedSession);

                selectedSession = null;
            }
            return;
        }

        private void createSession_Click(object sender, RoutedEventArgs e) {
            mainFrame = MainPage.getMainFrame();
            mainFrame.Navigate(typeof(UserLoggedIn.CreateSession));
        }

    }

}
