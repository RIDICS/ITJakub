using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Hangman.View
{
    public class FocusableTextBox : TextBox
    {
        public static readonly DependencyProperty IsFocusProperty = DependencyProperty.Register("IsFocus", typeof(bool), typeof(FocusableTextBox), new PropertyMetadata(default(bool), OnIsFocusChanged));

        public bool IsFocus
        {
            get { return (bool) GetValue(IsFocusProperty); }
            set { SetValue(IsFocusProperty, value); }
        }

        private static void OnIsFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as FocusableTextBox;
            if (textBox == null)
                return;

            var isActive = (bool) e.NewValue;

            if (isActive)
            {
                textBox.Focus(FocusState.Programmatic);
            }
        }
    }
}
