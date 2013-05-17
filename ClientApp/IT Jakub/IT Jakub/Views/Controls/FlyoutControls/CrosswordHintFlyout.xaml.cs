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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace IT_Jakub.Views.Controls.FlyoutControls {
    public sealed partial class CrosswordHintFlyout : UserControl {

        int start_x;
        int start_y;
        int horizontalWordLength = 0;
        int verticalWordLength = 0;

        internal CrosswordHintFlyout() {
            this.InitializeComponent();
        }

        internal CrosswordHintFlyout(string horizontalHint, string verticalHint) : this() {
            horizontal.Text = "Vodorovně:   " + horizontalHint;
            vertical.Text = "Svisle:   " + verticalHint;
        }

        internal CrosswordHintFlyout(string horizontalHint, string verticalHint, int start_x, int start_y)
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

        private void inputHorizontal_TextChanged(object sender, TextChangedEventArgs e) {
            CrosswordsApp.changeWord(inputHorizontal.Text, start_x, start_y, Direction.Horizontal);
        }

        private void inputVertical_TextChanged(object sender, TextChangedEventArgs e) {
            CrosswordsApp.changeWord(inputVertical.Text, start_x, start_y, Direction.Vertical);
        }

    }
}
