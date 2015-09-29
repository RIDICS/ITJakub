using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Hangman.View.Control
{
    public class NoLostFocusTextBox : TextBox
    {
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            
        }

        public void LostFocusProgrammatically()
        {
            base.OnLostFocus(new RoutedEventArgs());
        }
    }
}
