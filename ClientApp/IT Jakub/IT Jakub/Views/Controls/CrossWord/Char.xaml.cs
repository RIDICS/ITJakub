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

namespace IT_Jakub.Views.Controls.CrossWord {
    public sealed partial class Char : UserControl {
        public Char() {
            this.InitializeComponent();
        }

        public Char(string c) : this(){
            text.Text = c;
        }


        internal void changeText(char p) {
            text.Text = p.ToString();
        }

        internal string getText() {
            return text.Text.Trim();
        }
    }
}
