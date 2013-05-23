using Callisto.Controls;
using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Utils;
using IT_Jakub.Views.Controls.FlyoutControls;
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
    /// Page that shows an list of currently running sessions.
    /// </summary>
    public sealed partial class SessionsList : Page {


        private static ListView _sessionList;
        /// <summary>
        /// The selected session
        /// </summary>
        Session selectedSession = null;
        /// <summary>
        /// The ss is singleton instance of SignedSession where user is signed in.
        /// </summary>
        private static SignedSession ss = SignedSession.getInstance();
        /// <summary>
        /// The lu is singleton instance of LoggedUser. LoggedUser is user which is currently logged in.
        /// </summary>
        private static LoggedUser lu = LoggedUser.getInstance();

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionsList"/> class.
        /// </summary>
        public SessionsList() {
            this.InitializeComponent();
            _sessionList = sessionList;
        }
        
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e) {
            updateSessionList();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the sessionList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void sessionList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.RemovedItems.Count > 0) {
                selectedSession = null;
            }

            if (e.AddedItems.Count > 0) {
                selectedSession = (Session)e.AddedItems[0];
            }
            Views.Controls.BottomAppBar.setSelectedSession(selectedSession);
            Views.Controls.BottomAppBar.repaint(MainPage.getBottomAppBar());
        }

        /// <summary>
        /// Updates the session list.
        /// </summary>
        internal async static void updateSessionList() {
            try {
                SessionTable st = new SessionTable();
                SessionUserTable sut = new SessionUserTable();
                List<Session> allSessions = await st.getAllSessions();
                _sessionList.ItemsSource = allSessions;
            } catch (Exception ex) {
                object o = ex;
                return;
            }
        }

        /// <summary>
        /// Finds sessions containing the name string.
        /// </summary>
        /// <param name="name">The string that sessions must contain to display in list</param>
        internal async static void findSessionWithName(string name) {
            try {
                SessionTable st = new SessionTable();
                List<Session> items = await st.findSessionsByName(name);
                _sessionList.ItemsSource = items;
            } catch (Exception ex) {
                object o = ex;
                return;
            }
        }
    }
}
