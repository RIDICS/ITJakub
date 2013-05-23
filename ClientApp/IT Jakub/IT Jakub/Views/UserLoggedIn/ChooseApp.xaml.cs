using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Models.Commands;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IT_Jakub.Views.UserLoggedIn {
    /// <summary>
    /// Page where user can choose an type of application
    /// </summary>
    public sealed partial class ChooseApp : Page {

        /// <summary>
        /// The session data
        /// </summary>
        Session sessionData = null;
        /// <summary>
        /// The ss is singleton instance of SignedSession where user is signed in.
        /// </summary>
        private static SignedSession ss = SignedSession.getInstance();

        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseApp"/> class.
        /// </summary>
        public ChooseApp() {
            this.InitializeComponent();
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            sessionData = (Session)e.Parameter;
        }

        /// <summary>
        /// Handles the Click event of the SynchronizedReadingButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void SynchronizedReadingButton_Click(object sender, RoutedEventArgs e) {
            SessionTable st = new SessionTable();
            await st.createSession(sessionData);

            sessionData = await st.getSessionByName(sessionData.Name.Trim());
            ss.register(sessionData);
            await ss.sendCommand(CommandBuilder.getSyncReadingAppStartCommand());
            ss.login();
        }

        /// <summary>
        /// Handles the Click event of the CrosswordsButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void CrosswordsButton_Click(object sender, RoutedEventArgs e) {
            SessionTable st = new SessionTable();
            await st.createSession(sessionData);

            sessionData = await st.getSessionByName(sessionData.Name.Trim());
            ss.register(sessionData);
            await ss.sendCommand(CommandBuilder.getCrosswordAppStartCommand());
            ss.login();
        }
    }
}
