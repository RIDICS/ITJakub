﻿using IT_Jakub.Views.ApplicationStart;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IT_Jakub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        Page rootPage = null;
        private static Frame staticMainFrame;
        private static Views.Controls.BottomAppBar staticBottomAppBar;

        public MainPage()
        {
            this.InitializeComponent();
            staticMainFrame = mainFrame;
            staticBottomAppBar = bottomAppBar;
            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            rootPage = e.Parameter as Page;
            mainFrame.Navigate(typeof(ApplicationStart),this);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            if (mainFrame.CanGoBack) {
                mainFrame.GoBack();
            } else if (rootPage != null && rootPage.Frame.CanGoBack) {
                rootPage.Frame.GoBack();
            }
        }

        public static Frame getMainFrame() {
            return staticMainFrame;
        }

        private void MainButton_Click(object sender, RoutedEventArgs e) {
        }

        private void mainFrame_Navigated(object sender, NavigationEventArgs e) {
            BottomAppBar.IsOpen = false;
            TopAppBar.IsOpen = false;
        }

        internal static Views.Controls.BottomAppBar getBottomAppBar() {
            return staticBottomAppBar;
        }

        private void botBar_Opened(object sender, object e) {
            Views.Controls.BottomAppBar.repaint(bottomAppBar);
        }

    }
}
