using Callisto.Controls;
using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Utils;
using IT_Jakub.Views.Controls.FlyoutControls;
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
    /// Page used to create session.
    /// </summary>
    public sealed partial class CreateSession : Page {

        /// <summary>
        /// The lu is singleton instance of LoggedUser. LoggedUser is user which is currently logged in.
        /// </summary>
        private static LoggedUser lu = LoggedUser.getInstance();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSession"/> class.
        /// </summary>
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

        /// <summary>
        /// Handles the Click event of the createUserButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void createUserButton_Click(object sender, RoutedEventArgs e) {
            SessionTable st = new SessionTable();
            Session s = await st.getSessionByName(nameTextBox.Text.Trim());
            if (s != null && s.Name.Trim() == nameTextBox.Text.Trim()) {
                Flyout f = new Flyout();
                f.Content = new ErrorFlyout(
                    "Sezení již existuje",
                    "Sezení se jménem, které se snažíte vytvořit již existuje.\r\nZvolte prosím jiné jméno",
                    f
                    );
                f.PlacementTarget = this.Frame;
                f.Placement = PlacementMode.Top;
                f.IsOpen = true;
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
