using Callisto.Controls;
using IT_Jakub.Classes.Models;
using IT_Jakub.Views.EducationalApplications.SynchronizedReading;
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

namespace IT_Jakub.Views.Controls.FlyoutControls {

    /// <summary>
    /// Flyout shown while user decide to promote or demote user.
    /// </summary>
    public sealed partial class PromoteDemoteFlyout : UserControl {

        /// <summary>
        /// The selected user
        /// </summary>
        private User selectedUser;
        /// <summary>
        /// The ss is singleton instance of SignedSession where user is signed in.
        /// </summary>
        private static SignedSession ss = SignedSession.getInstance();
        /// <summary>
        /// The flyout
        /// </summary>
        private Flyout flyOut;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromoteDemoteFlyout"/> class.
        /// </summary>
        internal PromoteDemoteFlyout() {
            this.InitializeComponent();

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromoteDemoteFlyout"/> class.
        /// </summary>
        /// <param name="u">The user to be promoted/demoted</param>
        /// <param name="flyOut">The flyout.</param>
        internal PromoteDemoteFlyout(User u, Flyout flyOut) : this() {
            this.flyOut = flyOut;
            this.selectedUser = u;
            promote.IsEnabled = true;
            demote.IsEnabled = false;
            if (u.Id == ss.getSessionData().PrefferedUserId) {
                promote.IsEnabled = false;
                demote.IsEnabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the promote control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void promote_Click(object sender, RoutedEventArgs e) {
            if (selectedUser != null) {
                SyncReadingApp.promoteUser(selectedUser);
            }
            flyOut.IsOpen = false;
        }

        /// <summary>
        /// Handles the Click event of the demote control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void demote_Click(object sender, RoutedEventArgs e) {
            if (selectedUser != null) {
                SyncReadingApp.demoteUser(selectedUser);
            }
            flyOut.IsOpen = false;
        }
    }
}
