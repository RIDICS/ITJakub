using IT_Jakub.Classes.Models;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace IT_Jakub.Views.Controls.FlyoutControls {
    /// <summary>
    /// Flyout serves for password authentication of session.
    /// </summary>
    public sealed partial class PasswordSessionFlyout : UserControl {

        /// <summary>
        /// The ss is singleton instance of SignedSession where user is signed in.
        /// </summary>
        private static SignedSession ss = SignedSession.getInstance();

        /// <summary>
        /// The selected session
        /// </summary>
        private Session selectedSession;
        /// <summary>
        /// The flyout
        /// </summary>
        private Callisto.Controls.Flyout f;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordSessionFlyout"/> class.
        /// </summary>
        public PasswordSessionFlyout() {
            this.InitializeComponent();
            this.message.Text = "Sezení je chráněno heslem.\r\nZadejte prosím heslo pro přihlášení do sezení.";
            error.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordSessionFlyout"/> class.
        /// </summary>
        /// <param name="selectedSession">The selected session.</param>
        /// <param name="f">The f.</param>
        public PasswordSessionFlyout(Session selectedSession, Callisto.Controls.Flyout f) : this() {
            this.selectedSession = selectedSession;
            this.f = f;
        }

        /// <summary>
        /// Handles the Click event of the unlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void unlock_Click(object sender, RoutedEventArgs e) {
            if (selectedSession.Password == pwd.Text) {
                error.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                ss.register(selectedSession);
                ss.login();
                selectedSession = null;
                this.f.IsOpen = false;
            } else {
                error.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }
    }
}
