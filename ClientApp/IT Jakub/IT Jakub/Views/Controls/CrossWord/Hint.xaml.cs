using Callisto.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

namespace IT_Jakub.Views.Controls.CrossWord {
    /// <summary>
    /// Field where hint is stored
    /// </summary>
    public sealed partial class Hint : UserControl {

        /// <summary>
        /// The hint text
        /// </summary>
        string hintText;
        /// <summary>
        /// The start_x
        /// </summary>
        private int start_x;
        /// <summary>
        /// The start_y
        /// </summary>
        private int start_y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hint"/> class.
        /// </summary>
        public Hint() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hint"/> class.
        /// </summary>
        /// <param name="hintText">The hint text.</param>
        public Hint(string hintText)
            : this() {
            this.hintText = hintText;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hint"/> class.
        /// </summary>
        /// <param name="hintText">The hint text.</param>
        /// <param name="start_x">The start_x.</param>
        /// <param name="start_y">The start_y.</param>
        public Hint(string hintText, int start_x, int start_y)
            : this(hintText) {
            this.start_x = start_x;
            this.start_y = start_y;
        }

        /// <summary>
        /// Handles the Click event of the hint control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void hint_Click(object sender, RoutedEventArgs e) {
            Flyout f = new Flyout();
            f.Content = new FlyoutControls.CrosswordHintFlyout(hintText, start_x, start_y);
            f.PlacementTarget = this;
            f.Placement = PlacementMode.Bottom;
            f.IsOpen = true;
        }

        /// <summary>
        /// Gets the text of hint.
        /// </summary>
        /// <returns></returns>
        internal string getText() {
            return hintText;
        }
    }
}
