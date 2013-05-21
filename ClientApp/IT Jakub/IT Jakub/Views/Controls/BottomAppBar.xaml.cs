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

            if (currentPage.Contains("Views.UserLoggedIn")) {
                bar.userLoggedInTemplate();
            }

            if (currentPage.Contains("Views.ApplicationStart")) {
                bar.applicationStartTemplate();
            }

            if (currentPage.Contains("Views.UserLoggedIn.SessionsList")) {
                bar.sessionsListTemplate(bar);
            }

            if (currentPage.Contains("EducationalApplications")) {
                bar.educationalApplicationsTemplate();
            }
        }

        private void userLoggedInTemplate() {
            homeButton.Visibility = Visibility.Visible;
            logoutButton.Visibility = Visibility.Visible;

            signinSessionButton.Visibility = Visibility.Collapsed;
            createSession.Visibility = Visibility.Collapsed;
            deleteSession.Visibility = Visibility.Collapsed;
            findSession.Visibility = Visibility.Collapsed;
            signoutSessionButton.Visibility = Visibility.Collapsed;
            autoUpdateOnButton.Visibility = Visibility.Collapsed;
            autoUpdateOffButton.Visibility = Visibility.Collapsed;

            updateUserlistButton.Visibility = Visibility.Collapsed;
            highlightTextButton. Visibility = Visibility.Collapsed;
            openFileButton.Visibility = Visibility.Collapsed;
            
            requestFinalSolutions.Visibility = Visibility.Collapsed;
            sendFinalSolution.Visibility = Visibility.Collapsed;
            evaluateSolutions.Visibility = Visibility.Collapsed;
        }

        private void applicationStartTemplate() {
            homeButton.Visibility = Visibility.Visible;
            logoutButton.Visibility = Visibility.Collapsed;

            signinSessionButton.Visibility = Visibility.Collapsed;
            createSession.Visibility = Visibility.Collapsed;
            deleteSession.Visibility = Visibility.Collapsed;
            findSession.Visibility = Visibility.Collapsed;
            signoutSessionButton.Visibility = Visibility.Collapsed;
            autoUpdateOnButton.Visibility = Visibility.Collapsed;
            autoUpdateOffButton.Visibility = Visibility.Collapsed;

            updateUserlistButton.Visibility = Visibility.Collapsed;
            highlightTextButton.Visibility = Visibility.Collapsed;
            openFileButton.Visibility = Visibility.Collapsed;

            requestFinalSolutions.Visibility = Visibility.Collapsed;
            sendFinalSolution.Visibility = Visibility.Collapsed;
            evaluateSolutions.Visibility = Visibility.Collapsed;
        }

        private void sessionsListTemplate(BottomAppBar bar) {
            homeButton.Visibility = Visibility.Visible;
            logoutButton.Visibility = Visibility.Visible;

            signinSessionButton.Visibility = Visibility.Visible;

            if (lu.getUserData().Role == UserRole.Teacher || lu.getUserData().Role == UserRole.Principal) {
                createSession.Visibility = Visibility.Visible;
                deleteSession.Visibility = Visibility.Visible;
            }

            findSession.Visibility = Visibility.Visible;
            signoutSessionButton.Visibility = Visibility.Collapsed;
            autoUpdateOnButton.Visibility = Visibility.Collapsed;
            autoUpdateOffButton.Visibility = Visibility.Collapsed;

            updateUserlistButton.Visibility = Visibility.Collapsed;
            highlightTextButton.Visibility = Visibility.Collapsed;
            openFileButton.Visibility = Visibility.Collapsed;

            requestFinalSolutions.Visibility = Visibility.Collapsed;
            sendFinalSolution.Visibility = Visibility.Collapsed;
            evaluateSolutions.Visibility = Visibility.Collapsed;

            if (selectedSession != null) {
                signinSessionButton.IsEnabled = true;
                deleteSession.IsEnabled = true;
            } else {
                signinSessionButton.IsEnabled = false;
                deleteSession.IsEnabled = false;
            }
        }

        private void educationalApplicationsTemplate() {
            homeButton.Visibility = Visibility.Visible;
            logoutButton.Visibility = Visibility.Visible;

            signinSessionButton.Visibility = Visibility.Collapsed;
            createSession.Visibility = Visibility.Collapsed;
            deleteSession.Visibility = Visibility.Collapsed;
            findSession.Visibility = Visibility.Collapsed;
            signoutSessionButton.Visibility = Visibility.Visible;
            autoUpdateOnButton.Visibility = Visibility.Collapsed;
            autoUpdateOffButton.Visibility = Visibility.Collapsed;

            updateUserlistButton.Visibility = Visibility.Collapsed;
            highlightTextButton.Visibility = Visibility.Collapsed;
            openFileButton.Visibility = Visibility.Collapsed;

            requestFinalSolutions.Visibility = Visibility.Collapsed;
            sendFinalSolution.Visibility = Visibility.Collapsed;
            evaluateSolutions.Visibility = Visibility.Collapsed;

            if (currentPage.Contains("SyncReadingApp")) {
                syncReadingAppTemplate();
            }
            if (currentPage.Contains("Crosswords")) {
                crosswordsAppTemplate();
            }
        }

        private void syncReadingAppTemplate() {
            updateUserlistButton.Visibility = Visibility.Visible;
            highlightTextButton.Visibility = Visibility.Visible;
            openFileButton.Visibility = Visibility.Collapsed;
           
            if (EducationalApplications.SynchronizedReading.SyncReadingApp.isAutoUpdateEnabled()) {
                autoUpdateOnButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                autoUpdateOffButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            } else {
                autoUpdateOnButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                autoUpdateOffButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

            if (userRights == UserRights.Owner) {
                openFileButton.Visibility = Visibility.Visible;
                highlightTextButton.IsEnabled = true;
            }
            if (userRights == UserRights.Prefered) {
                highlightTextButton.IsEnabled = true;
            }
            if (userRights == UserRights.Default) {
                highlightTextButton.IsEnabled = false;
            }
        }

        private void crosswordsAppTemplate() {
            updateUserlistButton.Visibility = Visibility.Collapsed;
            highlightTextButton.Visibility = Visibility.Collapsed;
            openFileButton.Visibility = Visibility.Collapsed;
            autoUpdateOnButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            autoUpdateOffButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            requestFinalSolutions.Visibility = Visibility.Collapsed;
            sendFinalSolution.Visibility = Visibility.Visible;
            evaluateSolutions.Visibility = Visibility.Collapsed;
            
            if (userRights == UserRights.Owner) {
                openFileButton.Visibility = Visibility.Visible;
                requestFinalSolutions.Visibility = Visibility.Visible;
                evaluateSolutions.Visibility = Visibility.Visible;
            }
        }

        private void navigateToLoginPage() {
            mainFrame = MainPage.getMainFrame();
            mainFrame.Navigate(typeof(Views.ApplicationStart.ApplicationStart));
        }

        private async void logoutUser() {
            selectedSession = null;
            await lu.logout();
            mainFrame = MainPage.getMainFrame();
            mainFrame.Navigate(typeof(ApplicationStart.ApplicationStart));
        }

        private async void signOutSession() {
            selectedSession = null;
            if (ss.isSignedInSession()) {
                await ss.signout();
            }
            mainFrame = MainPage.getMainFrame();
            mainFrame.Navigate(typeof(UserLoggedIn.SessionsList));
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
            mainFrame = MainPage.getMainFrame();
            if (lu.isLoggedIn()) {
                signOutSession();
                return;
            }
            mainFrame.Navigate(typeof(MainPage));
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

        private void requestFinalSolutions_Click(object sender, RoutedEventArgs e) {
            Views.EducationalApplications.Crosswords.CrosswordsApp.requestFinalSolutions();
        }

        private void sendFinalSolution_Click(object sender, RoutedEventArgs e) {
            Views.EducationalApplications.Crosswords.CrosswordsApp.sendFinalSolution();
        }

        private void evaluateSolutions_Click(object sender, RoutedEventArgs e) {
            TaskKiller.killEducationalApplicationTasks();
            Views.EducationalApplications.Crosswords.CrosswordsApp.evaluateSolutions();
        }

    }

}
