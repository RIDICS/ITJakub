using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ITJakub.MobileApps.Client.Hangman.View
{
    public class LazyTextBox : TextBox
    {
        public LazyTextBox()
        {
         //   IsTapEnabled = false;
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            //base.OnTapped(e);
        }
    }
}
