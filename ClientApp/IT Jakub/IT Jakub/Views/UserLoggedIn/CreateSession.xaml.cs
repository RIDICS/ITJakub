using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Utils;
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
    public sealed partial class CreateSession : Page {

        private static LoggedUser lu = LoggedUser.getInstance();

        public CreateSession() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
        }

        private async void createUserButton_Click(object sender, RoutedEventArgs e) {
            SessionTable st = new SessionTable();
            Session s = await st.getSessionByName(nameTextBox.Text.Trim());
            if (s != null && s.Name.Trim() == nameTextBox.Text.Trim()) {
                MyDialogs.showDialogOK("Sezení s tímto jménem již existuje, zvolte prosím jiné jméno");
                return;
            }
            Session newSession = new Session {
                Name = nameTextBox.Text.Trim(),
                Password = passwordTextBox.Text.Trim(),
                OwnerUserId = lu.getUserData().Id,
                PrefferedUserId = 0,
                SessionUpdateId = 0
            };
            this.Frame.Navigate(typeof(ChooseApp),newSession);
        }
    }
}
