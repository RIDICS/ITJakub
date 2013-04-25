using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Models.Commands;
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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChooseApp : Page {

        Session sessionData = null;
        private static SignedSession ss = SignedSession.getInstance();

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
        
        private async void SynchronizedReadingButton_Click(object sender, RoutedEventArgs e) {
            SessionTable st = new SessionTable();
            await st.createSession(sessionData);

            sessionData = await st.getSessionByName(sessionData.Name.Trim());
            ss.register(sessionData);
            await ss.sendCommand(Command.SYNCHRONIZED_READING_APPLICATION + ':' + SyncReadingAppCommand.WHOLE_APPLICATION + ':' + SyncReadingAppCommand.START_APPLICATION);
            ss.login();
        }
    }
}
