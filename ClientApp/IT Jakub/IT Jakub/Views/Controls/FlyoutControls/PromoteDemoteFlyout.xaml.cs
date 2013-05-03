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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace IT_Jakub.Views.Controls.FlyoutControls {
    public sealed partial class PromoteDemoteFlyout : UserControl {
        
        private User selectedUser;
        private static SignedSession ss = SignedSession.getInstance();
        private User u;
        private Flyout flyOut;

        internal PromoteDemoteFlyout() {
            this.InitializeComponent();

        }

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

        private void promote_Click(object sender, RoutedEventArgs e) {
            if (selectedUser != null) {
                SyncReadingApp.promoteUser(selectedUser);
            }
            flyOut.IsOpen = false;
        }

        private void demote_Click(object sender, RoutedEventArgs e) {
            if (selectedUser != null) {
                SyncReadingApp.demoteUser(selectedUser);
            }
            flyOut.IsOpen = false;
        }
    }
}
