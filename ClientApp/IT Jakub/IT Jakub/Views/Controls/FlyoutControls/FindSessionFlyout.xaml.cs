using Callisto.Controls;
using IT_Jakub.Views.EducationalApplications.SynchronizedReading;
using IT_Jakub.Views.EducationalApplications.Crosswords;
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
using Windows.Storage.Pickers;
using Windows.Storage;

namespace IT_Jakub.Views.Controls.FlyoutControls {

    /// <summary>
    /// Flayout shown while user decides to open new file from URI
    /// </summary>
    public sealed partial class FindSessionFlyout : UserControl {

        /// <summary>
        /// The flyout
        /// </summary>
        private Flyout flyOut;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindSessionFlyout"/> class.
        /// </summary>
        public FindSessionFlyout(Flyout f) {
            this.InitializeComponent();
            flyOut = f;
        }

        /// <summary>
        /// Handles the Click event of the findSessionButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void findSessionButton_Click(object sender, RoutedEventArgs e) {
            Views.UserLoggedIn.SessionsList.findSessionWithName(name.Text);
            flyOut.IsOpen = false;
        }

    }
}
