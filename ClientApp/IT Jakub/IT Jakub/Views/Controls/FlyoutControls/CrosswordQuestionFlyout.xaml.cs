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
    public sealed partial class CrosswordQuestionFlyout : UserControl {

        /// <summary>
        /// The start_x horizontal position
        /// </summary>
        int start_x;
        /// <summary>
        /// The start_x vertical position
        /// </summary>
        int start_y;
        /// <summary>
        /// The horizontal word length
        /// </summary>
        int horizontalWordLength = 0;
        /// <summary>
        /// The vertical word length
        /// </summary>
        int verticalWordLength = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrosswordQuestionFlyout"/> class.
        /// </summary>
        internal CrosswordQuestionFlyout() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrosswordQuestionFlyout"/> class.
        /// </summary>
        /// <param name="horizontalHint">The horizontal hint.</param>
        /// <param name="verticalHint">The vertical hint.</param>
        internal CrosswordQuestionFlyout(string horizontalHint, string verticalHint) : this() {
            horizontal.Text = "Vodorovně:   " + horizontalHint;
            vertical.Text = "Svisle:   " + verticalHint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrosswordQuestionFlyout"/> class.
        /// </summary>
        /// <param name="horizontalHint">The horizontal hint.</param>
        /// <param name="verticalHint">The vertical hint.</param>
        /// <param name="start_x">The start_x horizontal position</param>
        /// <param name="start_y">The start_y vertical  position</param>
        internal CrosswordQuestionFlyout(string horizontalHint, string verticalHint, int start_x, int start_y)
            : this(horizontalHint, verticalHint) {
            this.start_x = start_x;
            this.start_y = start_y;

            ScrollViewer s = CrosswordsApp.getCrossWord();
            var sv = s.Content as StackPanel;
           
            while (true) {
                for (int i = 1; i <= sv.Children.Count; i++) {
                    var col = sv.Children[start_x + i] as StackPanel;
                    if (col != null) {
                        var cell = col.Children[start_y];
                        if (cell.GetType() == typeof(IT_Jakub.Views.Controls.CrossWord.Char)) {
                            horizontalWordLength = i;
                            continue;
                        } else {
                            break;
                        }
                    } else {
                        break;
                    }
                }
                break;
            }

            var collum = sv.Children[start_x] as StackPanel;
            while (true) {
                for (int i = 1; i <= collum.Children.Count; i++) {
                    var cell = collum.Children[start_y + i];
                    if (cell != null) {
                        if (cell.GetType() == typeof(IT_Jakub.Views.Controls.CrossWord.Char)) {
                            verticalWordLength = i;
                            continue;
                        } else {
                            break;
                        }
                    }
                }
                break;
            }

            inputVertical.MaxLength = verticalWordLength;
            inputHorizontal.MaxLength = horizontalWordLength;

            if (verticalWordLength == 0) {
                inputVertical.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                vertical.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            if (horizontalWordLength == 0) {
                inputHorizontal.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                horizontal.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

        }

        /// <summary>
        /// Handles the TextChanged event of the inputHorizontal control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void inputHorizontal_TextChanged(object sender, TextChangedEventArgs e) {
            CrosswordsApp.changeWord(inputHorizontal.Text, start_x, start_y, Direction.Horizontal);
        }

        /// <summary>
        /// Handles the TextChanged event of the inputVertical control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void inputVertical_TextChanged(object sender, TextChangedEventArgs e) {
            CrosswordsApp.changeWord(inputVertical.Text, start_x, start_y, Direction.Vertical);
        }

    }
}
