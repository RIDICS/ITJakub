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
    /// <summary>
    /// Control of BottomAppBar
    /// </summary>
    public sealed partial class BottomAppBar : UserControl {

        /// <summary>
        /// The lu is singleton instance of LoggedUser. LoggedUser is user which is currently logged in.
        /// </summary>
        private static LoggedUser lu = LoggedUser.getInstance();
        /// <summary>
        /// The ss is singleton instance of SignedSession where user is signed in.
        /// </summary>
        private static SignedSession ss = SignedSession.getInstance();
        /// <summary>
        /// The main frame of every page
        /// </summary>
        private Frame mainFrame;
        /// <summary>
        /// The selected session from sessionlist
        /// </summary>
        private static Session selectedSession;
        /// <summary>
        /// The current navigated page
        /// </summary>
        private static string currentPage;
        /// <summary>
        /// The user rights
        /// </summary>
        private static UserRights userRights = UserRights.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="BottomAppBar"/> class.
        /// </summary>
        public BottomAppBar() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Repaints the specified bar.
        /// </summary>
        /// <param name="bar">The bar to be repainted</param>
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

            if (currentPage.Contains("Views.UserLoggedIn.CreateSession")) {
                bar.createSessionTemplate(bar);
            }

            if (currentPage.Contains("Views.UserLoggedIn.ChooseApp")) {
                bar.chooseAppTemplate(bar);
            }

            if (currentPage.Contains("EducationalApplications")) {
                bar.educationalApplicationsTemplate();
            }
        }

        

        /// <summary>
        /// Users logged in template.
        /// </summary>
        private void userLoggedInTemplate() {
            backButton.Visibility = Visibility.Collapsed;
            homeButton.Visibility = Visibility.Visible;
            logoutButton.Visibility = Visibility.Visible;

            signinSessionButton.Visibility = Visibility.Collapsed;
            createSession.Visibility = Visibility.Collapsed;
            deleteSession.Visibility = Visibility.Collapsed;
            updateSessionList.Visibility = Visibility.Collapsed;
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

        /// <summary>
        /// Application start template.
        /// </summary>
        private void applicationStartTemplate() {
            backButton.Visibility = Visibility.Collapsed;
            homeButton.Visibility = Visibility.Visible;
            logoutButton.Visibility = Visibility.Collapsed;

            signinSessionButton.Visibility = Visibility.Collapsed;
            createSession.Visibility = Visibility.Collapsed;
            deleteSession.Visibility = Visibility.Collapsed;
            updateSessionList.Visibility = Visibility.Collapsed;
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

        /// <summary>
        /// Sessions list template.
        /// </summary>
        /// <param name="bar">The bar.</param>
        private void sessionsListTemplate(BottomAppBar bar) {
            backButton.Visibility = Visibility.Collapsed;
            homeButton.Visibility = Visibility.Visible;
            logoutButton.Visibility = Visibility.Visible;

            signinSessionButton.Visibility = Visibility.Visible;

            if (lu.getUserData().Role == (int)UserRole.Teacher || lu.getUserData().Role == (int)UserRole.Principal) {
                createSession.Visibility = Visibility.Visible;
                deleteSession.Visibility = Visibility.Visible;
            }

            updateSessionList.Visibility = Visibility.Visible;
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

        /// <summary>
        /// Create session template.
        /// </summary>
        /// <param name="bar">The bar.</param>
        private void createSessionTemplate(BottomAppBar bar) {
            backButton.Visibility = Visibility.Visible;
            homeButton.Visibility = Visibility.Visible;
            logoutButton.Visibility = Visibility.Visible;

            signinSessionButton.Visibility = Visibility.Collapsed;
            createSession.Visibility = Visibility.Collapsed;
            deleteSession.Visibility = Visibility.Collapsed;
            updateSessionList.Visibility = Visibility.Collapsed;
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

        /// <summary>
        /// Choose app template.
        /// </summary>
        /// <param name="bar">The bar.</param>
        private void chooseAppTemplate(BottomAppBar bar) {
            backButton.Visibility = Visibility.Visible;
            homeButton.Visibility = Visibility.Visible;
            logoutButton.Visibility = Visibility.Visible;

            signinSessionButton.Visibility = Visibility.Collapsed;
            createSession.Visibility = Visibility.Collapsed;
            deleteSession.Visibility = Visibility.Collapsed;
            updateSessionList.Visibility = Visibility.Collapsed;
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

        /// <summary>
        /// Educational applications template.
        /// </summary>
        private void educationalApplicationsTemplate() {
            backButton.Visibility = Visibility.Collapsed;
            homeButton.Visibility = Visibility.Visible;
            logoutButton.Visibility = Visibility.Visible;

            signinSessionButton.Visibility = Visibility.Collapsed;
            createSession.Visibility = Visibility.Collapsed;
            deleteSession.Visibility = Visibility.Collapsed;
            updateSessionList.Visibility = Visibility.Collapsed;
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

        /// <summary>
        /// Syncs reading app template.
        /// </summary>
        private void syncReadingAppTemplate() {
            backButton.Visibility = Visibility.Collapsed;
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

        /// <summary>
        /// Crosswords app template.
        /// </summary>
        private void crosswordsAppTemplate() {
            backButton.Visibility = Visibility.Collapsed;
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

            if (currentPage.Contains("EvaluateSolutions")) {
                backButton.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Navigates to login page.
        /// </summary>
        private void navigateToLoginPage() {
            mainFrame = MainPage.getMainFrame();
            mainFrame.Navigate(typeof(Views.ApplicationStart.ApplicationStart));
        }

        /// <summary>
        /// Logout the user.
        /// </summary>
        private async void logoutUser() {
            selectedSession = null;
            await lu.logout();
            mainFrame = MainPage.getMainFrame();
            mainFrame.Navigate(typeof(ApplicationStart.ApplicationStart));
        }

        /// <summary>
        /// Signs out user from session.
        /// </summary>
        private async void signOutSession() {
            selectedSession = null;
            if (ss.isSignedInSession()) {
                await ss.signout();
            }
            mainFrame = MainPage.getMainFrame();
            mainFrame.Navigate(typeof(UserLoggedIn.SessionsList));
        }

        /// <summary>
        /// Signs in user in session.
        /// </summary>
        private void signInSession() {
            if (selectedSession != null) {
                if (selectedSession.Password == null) {
                    ss.register(selectedSession);
                    ss.login();
                } else {
                    Flyout f = new Flyout();
                    f.Content = new PasswordSessionFlyout(selectedSession, f);
                    f.PlacementTarget = signinSessionButton;
                    f.Placement = PlacementMode.Top;
                    f.IsOpen = true;
                }
                selectedSession = null;
            }
            return;
        }

        /// <summary>
        /// Sets the selected session.
        /// </summary>
        /// <param name="s">The s.</param>
        internal static void setSelectedSession(Session s) {
            selectedSession = s;
        }

        /// <summary>
        /// Handles the Click event of the homeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void homeButton_Click(object sender, RoutedEventArgs e) {
            mainFrame = MainPage.getMainFrame();
            if (lu.isLoggedIn()) {
                signOutSession();
                return;
            }
            mainFrame.Navigate(typeof(MainPage));
        }

        /// <summary>
        /// Handles the Click event of the logoutButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void logoutButton_Click(object sender, RoutedEventArgs e) {
            logoutUser();
        }

        /// <summary>
        /// Handles the Click event of the signinSessionButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void signinSessionButton_Click(object sender, RoutedEventArgs e) {
            signInSession();
        }

        /// <summary>
        /// Handles the Click event of the signoutSessionButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void signoutSessionButton_Click(object sender, RoutedEventArgs e) {
            signOutSession();
        }

        /// <summary>
        /// Handles the Click event of the autoUpdateOnButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void autoUpdateOnButton_Click(object sender, RoutedEventArgs e) {
            EducationalApplications.SynchronizedReading.SyncReadingApp.setAutoUpdate(true);
            repaint(this);
        }

        /// <summary>
        /// Handles the Click event of the autoUpdateOffButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void autoUpdateOffButton_Click(object sender, RoutedEventArgs e) {
            EducationalApplications.SynchronizedReading.SyncReadingApp.setAutoUpdate(false);
            repaint(this);
        }

        /// <summary>
        /// Handles the Click event of the highlightTextButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void highlightTextButton_Click(object sender, RoutedEventArgs e) {
            SyncReadingApp.highligtTextButton_Click();
        }

        /// <summary>
        /// Handles the Click event of the updateUserlistButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void updateUserlistButton_Click(object sender, RoutedEventArgs e) {
            SyncReadingApp.updateUserList();
        }

        /// <summary>
        /// Sets the user rights.
        /// </summary>
        /// <param name="ur">The ur.</param>
        public static void setUserRights (UserRights ur) {
            userRights = ur;
        }

        /// <summary>
        /// Handles the Click event of the openFileButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void openFileButton_Click(object sender, RoutedEventArgs e) {
            Flyout flyOut = new Flyout();
            flyOut.Content = new OpenFileFlyout(flyOut);
            flyOut.PlacementTarget = sender as UIElement;
            flyOut.Placement = PlacementMode.Top;
            flyOut.Width = 400;
            flyOut.IsOpen = true;
        }

        /// <summary>
        /// Handles the Click event of the deleteSession control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void deleteSession_Click(object sender, RoutedEventArgs e) {
            if (selectedSession != null) {
                if (selectedSession.OwnerUserId == lu.getUserData().Id) {
                    SessionTable st = new SessionTable();
                    st.removeSession(selectedSession);

                    SessionUserTable sut = new SessionUserTable();
                    await sut.removeAllUsersFromSession(selectedSession);

                    CommandTable ct = new CommandTable();
                    await ct.removeSessionsCommand(selectedSession);

                    selectedSession = null;
                } else {
                    Flyout f = new Flyout();
                    f.Content = new ErrorFlyout("Nedostatečná oprávnění !", "Sezení vytvořil jiný uživatel.\r\nNemáte dostatečná oprávnění k jeho smazání.", f);
                    f.PlacementTarget = MainPage.getMainFrame();
                    f.Placement = PlacementMode.Top;
                    f.IsOpen = true;
                }
            }
            Views.UserLoggedIn.SessionsList.updateSessionList();
        }

        /// <summary>
        /// Handles the Click event of the createSession control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void createSession_Click(object sender, RoutedEventArgs e) {
            mainFrame = MainPage.getMainFrame();
            mainFrame.Navigate(typeof(UserLoggedIn.CreateSession));
        }

        /// <summary>
        /// Handles the Click event of the requestFinalSolutions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void requestFinalSolutions_Click(object sender, RoutedEventArgs e) {
            Views.EducationalApplications.Crosswords.CrosswordsApp.requestFinalSolutions();
        }

        /// <summary>
        /// Handles the Click event of the sendFinalSolution control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void sendFinalSolution_Click(object sender, RoutedEventArgs e) {
            Views.EducationalApplications.Crosswords.CrosswordsApp.sendFinalSolution();
        }

        /// <summary>
        /// Handles the Click event of the evaluateSolutions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void evaluateSolutions_Click(object sender, RoutedEventArgs e) {
            TaskKiller.killEducationalApplicationTasks();
            Views.EducationalApplications.Crosswords.CrosswordsApp.evaluateSolutions();
        }

        /// <summary>
        /// Handles the Click event of the backButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void backButton_Click(object sender, RoutedEventArgs e) {
            MainPage.goBack();
        }

        /// <summary>
        /// Handles the Click event of the findSession control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void findSession_Click(object sender, RoutedEventArgs e) {
            Flyout f = new Flyout();
            f.Content = new FindSessionFlyout(f);
            f.PlacementTarget = findSession;
            f.Placement = PlacementMode.Top;
            f.IsOpen = true;
        }

        /// <summary>
        /// Handles the Click event of the updateSessionList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void updateSessionList_Click(object sender, RoutedEventArgs e) {
            Views.UserLoggedIn.SessionsList.updateSessionList();
        }

    }

}
