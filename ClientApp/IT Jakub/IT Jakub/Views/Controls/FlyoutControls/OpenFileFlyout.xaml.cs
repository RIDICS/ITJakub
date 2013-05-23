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
    public sealed partial class OpenFileFlyout : UserControl {

        /// <summary>
        /// The flyout
        /// </summary>
        private Flyout flyOut;
        /// <summary>
        /// The current page
        /// </summary>
        private static string currentPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileFlyout"/> class.
        /// </summary>
        public OpenFileFlyout() {
            this.InitializeComponent();
            Frame mainFrame = MainPage.getMainFrame();
            currentPage = mainFrame.CurrentSourcePageType.FullName;
            if (currentPage.Contains("Views.EducationalApplications.SynchronizedReading")) {
                openFileButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileFlyout"/> class.
        /// </summary>
        /// <param name="flyOut">The fly out.</param>
        public OpenFileFlyout(Flyout flyOut) : this() {
            this.flyOut = flyOut;
        }

        /// <summary>
        /// Handles the Click event of the openButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void openUriButton_Click(object sender, RoutedEventArgs e) {
            Frame mainFrame = MainPage.getMainFrame();
            currentPage = mainFrame.CurrentSourcePageType.FullName;
            if (currentPage.Contains("Views.EducationalApplications.SynchronizedReading")) {
                SyncReadingApp.openFile(uri.Text.Trim());
            }
            if (currentPage.Contains("Views.EducationalApplications.Crosswords")) {
                CrosswordsApp.openFileFromUri(uri.Text.Trim());
            }
            flyOut.IsOpen = false;
        }

        private async void openFileButton_Click(object sender, RoutedEventArgs e) {
            Frame mainFrame = MainPage.getMainFrame();
            currentPage = mainFrame.CurrentSourcePageType.FullName;
            if (currentPage.Contains("Views.EducationalApplications.SynchronizedReading")) {
            }
            if (currentPage.Contains("Views.EducationalApplications.Crosswords")) {
                FileOpenPicker fop = new FileOpenPicker();
                fop.FileTypeFilter.Add(".xml");
                StorageFile f = await fop.PickSingleFileAsync();
                CrosswordsApp.openFileFromLocalStorage(f);
            }
            flyOut.IsOpen = false;
        }
    }
}
