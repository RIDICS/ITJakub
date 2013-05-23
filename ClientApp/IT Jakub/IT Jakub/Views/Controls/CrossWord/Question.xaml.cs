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
    
    public enum Direction {
        Horizontal = 0,
        Vertical = 1
    }

    public sealed partial class Question : UserControl {

        string horizontalHintText;
        string verticalHintText;
        private int start_x;
        private int start_y;

        public Question() {
            this.InitializeComponent();
        }

        public Question(string hintText)
            : this() {
                Regex r = new Regex(Regex.Escape("]:[") + "(.*)$");
                this.horizontalHintText = r.Replace(hintText, "");
                this.horizontalHintText = this.horizontalHintText.Remove(0,1);

                r = new Regex("^(.*)" + Regex.Escape("]:["));
                this.verticalHintText = r.Replace(hintText, "");
                this.verticalHintText = this.verticalHintText.Remove(verticalHintText.Length-1, 1);
        }

        public Question(string hintText, int start_x, int start_y)
            : this(hintText) {
            this.start_x = start_x;
            this.start_y = start_y;
        }

        private void hint_Click(object sender, RoutedEventArgs e) {
            Flyout f = new Flyout();
            f.Content = new FlyoutControls.CrosswordQuestionFlyout(horizontalHintText, verticalHintText, start_x, start_y);
            f.PlacementTarget = this;
            f.Placement = PlacementMode.Bottom;
            f.IsOpen = true;
        }

        internal string getHorizontalHint() {
            return horizontalHintText;
        }

        internal string getVerticalHint() {
            return verticalHintText;
        }

    }
}
