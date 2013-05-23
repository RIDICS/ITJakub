using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace IT_Jakub.Views.Controls.CrossWord {
    public sealed partial class Char : UserControl {
        private bool isPuzzle;

        public Char() {
            this.InitializeComponent();
        }

        public Char(string c) : this(){
            text.Text = c;
        }

        public Char(string text, bool isPuzzle) : this(text) {
            this.isPuzzle = isPuzzle;
            this.text.Background = new SolidColorBrush(Color.FromArgb(255, 180, 200, 255));
        }

        internal bool isPuzzleChar() {
            return isPuzzle;
        }

        internal void changeText(char p) {
            text.Text = p.ToString();
        }

        internal string getText() {
            return text.Text.Trim();
        }
    }
}
