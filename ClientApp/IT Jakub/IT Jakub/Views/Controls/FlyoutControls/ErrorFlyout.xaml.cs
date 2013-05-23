using Callisto.Controls;
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
    /// Flyout shown as error.
    /// </summary>
    public sealed partial class ErrorFlyout : UserControl {

        /// <summary>
        /// The flyout
        /// </summary>
        private Flyout f;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorFlyout"/> class.
        /// </summary>
        /// <param name="heading">The heading.</param>
        /// <param name="message">The message.</param>
        /// <param name="flyout">The flyout.</param>
        public ErrorFlyout(string heading, string message, Flyout flyout) {
            this.InitializeComponent();
            this.heading.Text = heading;
            this.message.Text = message;
            this.f = flyout;
        }

        /// <summary>
        /// Handles the Click event of the close control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void close_Click(object sender, RoutedEventArgs e) {
            f.IsOpen = false;
        }
    }
}
