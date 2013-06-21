using IT_Jakub.Classes.Authentication;
using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Utils;

using IT_Jakub.Views.UserManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace IT_Jakub.Views.ApplicationStart {
    /// <summary>
    /// Page shows a about page.
    /// </summary>
    public sealed partial class About : Page {

        /// <summary>
        /// Initializes a new instance of the <see cref="About"/> class.
        /// </summary>
        public About() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
        }
        
        private void back_Click(object sender, RoutedEventArgs e) {
            MainPage.goBack();
        }

    }
}
