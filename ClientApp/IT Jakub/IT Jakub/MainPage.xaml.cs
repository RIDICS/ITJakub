using IT_Jakub.Views.ApplicationStart;
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
    /// Main page where all common components are designed
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// The root page
        /// </summary>
        Page rootPage = null;
        /// <summary>
        /// The mainFrame
        /// </summary>
        private static Frame staticMainFrame;
        /// <summary>
        /// The bottom app bar
        /// </summary>
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
            mainFrame.Navigate(typeof(ApplicationStart));
        }

        /// <summary>
        /// Goes back.
        /// </summary>
        /// 
        public static void goBack() {
            if (staticMainFrame.CanGoBack) {
                staticMainFrame.GoBack();
            }
        }

        /// <summary>
        /// Gets the main frame.
        /// </summary>
        /// <returns></returns>
        public static Frame getMainFrame() {
            return staticMainFrame;
        }

        /// <summary>
        /// Handles the Click event of the MainButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MainButton_Click(object sender, RoutedEventArgs e) {
        }

        /// <summary>
        /// Handles the Navigated event of the mainFrame control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        private void mainFrame_Navigated(object sender, NavigationEventArgs e) {
            BottomAppBar.IsOpen = false;
        }

        /// <summary>
        /// Gets the bottom app bar.
        /// </summary>
        /// <returns></returns>
        internal static Views.Controls.BottomAppBar getBottomAppBar() {
            return staticBottomAppBar;
        }

        /// <summary>
        /// Bots the bar_ opened.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void botBar_Opened(object sender, object e) {
            Views.Controls.BottomAppBar.repaint(bottomAppBar);
        }

    }
}
