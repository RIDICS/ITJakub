// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace ITJakub.MobileApps.Client.Crosswords.View
{
    public sealed partial class SimpleCrosswordsControl
    {
        public SimpleCrosswordsControl()
        {
            InitializeComponent();
        }

        private void WordTextBox_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter)
                return;

            var textBox = (TextBox) sender;
            var flyoutPresenter = (FlyoutPresenter) textBox.Parent;
            var popup = (Popup) flyoutPresenter.Parent;
            popup.IsOpen = false;
        }
    }
}
