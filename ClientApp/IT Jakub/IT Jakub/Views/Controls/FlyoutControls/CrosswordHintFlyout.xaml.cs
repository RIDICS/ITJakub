using Callisto.Controls;
using IT_Jakub.Classes.Models;
using IT_Jakub.Views.Controls.CrossWord;
using IT_Jakub.Views.EducationalApplications.Crosswords;
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

namespace IT_Jakub.Views.Controls.FlyoutControls {
    /// <summary>
    /// Control of flyout that shows on field click that Crossword app use to show hint in particular crossword
    /// </summary>
    public sealed partial class CrosswordHintFlyout : UserControl {

        /// <summary>
        /// The start_x horizontal position
        /// </summary>
        int start_x;
        /// <summary>
        /// The start_x vertical position
        /// </summary>
        int start_y;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrosswordHintFlyout" /> class.
        /// </summary>
        internal CrosswordHintFlyout() {
            this.InitializeComponent();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CrosswordHintFlyout"/> class.
        /// </summary>
        /// <param name="hintText">The hint text.</param>
        internal CrosswordHintFlyout(string hintText) : this() {
            hint.Text = hintText;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CrosswordHintFlyout"/> class.
        /// </summary>
        /// <param name="hintText">The hint text.</param>
        /// <param name="start_x">The start_x.</param>
        /// <param name="start_y">The start_y.</param>
        internal CrosswordHintFlyout(string hintText, int start_x, int start_y)
            : this(hintText) {
            this.start_x = start_x;
            this.start_y = start_y;
        }

    }
}
