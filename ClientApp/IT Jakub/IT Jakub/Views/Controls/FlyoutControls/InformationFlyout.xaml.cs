using Callisto.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    /// Flyout that is shown as information to user.
    /// </summary>
    public sealed partial class InformationFlyout : UserControl {

        /// <summary>
        /// The flyout
        /// </summary>
        Flyout f;

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationFlyout"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="f">The flyout</param>
        public InformationFlyout(string message, Flyout f) {
            this.InitializeComponent();
            this.f = f;
            this.text.Text = message;
        }
    }
}
