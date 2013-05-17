﻿using Callisto.Controls;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace IT_Jakub.Views.Controls.FlyoutControls {
    public sealed partial class OpenFileFlyout : UserControl {
        
        private Flyout flyOut;
        private static string currentPage;
        
        public OpenFileFlyout() {
            this.InitializeComponent();
        }

        public OpenFileFlyout(Flyout flyOut) : this() {
            this.flyOut = flyOut;
        }
        
        private void openButton_Click(object sender, RoutedEventArgs e) {
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
    }
}
