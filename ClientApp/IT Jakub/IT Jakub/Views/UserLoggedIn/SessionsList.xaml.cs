using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Utils;
using Microsoft.WindowsAzure.MobileServices;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IT_Jakub.Views.UserLoggedIn {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SessionsList : Page {

        Session selectedSession = null;
        private static SignedSession ss = SignedSession.getInstance();
        private static LoggedUser lu = LoggedUser.getInstance();

        public SessionsList() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            sessionListUpdate_Click(sessionListUpdate, null);
        }

        private async void sessionListUpdate_Click(object sender, RoutedEventArgs e) {
            SessionTable st = new SessionTable();
            SessionUserTable sut = new SessionUserTable();
            List<SessionUser> list = await sut.getAllSessionUsers();
            try {
                MobileServiceCollectionView<Session> allSessions = st.getAllSessions();
                sessionList.ItemsSource = allSessions;
                /* Neodchycena vyjímka: 
                 * Microsoft.WindowsAzure.MobileServices.MobileServiceInvalidOperationException was unhandled
                 * The request could not be completed.  (NameResolutionFailure)
                 * 
                 * Objevuje se při pokusu o vylistování uživatelů v offline režimu.
                 * 
                 */
            } catch (Exception ex) {
                MyDialogs.showDialogOK(ex.Message);
                return;
            }
        }

        private async void sessionList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.RemovedItems.Count > 0) {
                selectedSession = null;
            }

            if (e.AddedItems.Count > 0) {
                selectedSession = (Session)e.AddedItems[0];
                CommandTable ct = new CommandTable();
                List<Command> cl = await ct.getAllSessionCommands(selectedSession);
                cl = await ct.getAllCommands();
            }
        }

        private void joinChossenSession_Click(object sender, RoutedEventArgs e) {
            if (selectedSession != null) {
                ss.register(selectedSession);
                ss.login();
            }
            return;
        }

        private void createSession_Click(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(CreateSession));
        }

        private async void removeSelectedSessionButton_Click(object sender, RoutedEventArgs e) {
            if (selectedSession != null) {
                SessionTable st = new SessionTable();
                st.removeSession(selectedSession);
                CommandTable ct = new CommandTable();
                await ct.removeSessionsCommand(selectedSession);
                SessionUserTable sut = new SessionUserTable();
                await sut.removeAllUsersFromSession(selectedSession);
            }
            sessionListUpdate_Click(sender, null);
            
        }
    }
}
