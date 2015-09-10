using System.Windows.Input;
using Windows.UI.Xaml;

namespace ITJakub.MobileApps.Client.Shared.Control
{
    public class PasswordBoxBehaviors
    {
        public static readonly DependencyProperty EnterCommandProperty =
             DependencyProperty.RegisterAttached("EnterCommand", typeof(ICommand),
                 typeof(FocusableBehavior), new PropertyMetadata(false, OnIsFocusPropertyChanged));

        public static void SetEnterCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(EnterCommandProperty, value);
        }

        public static ICommand GetEnterCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(EnterCommandProperty);
        }

        private static void OnIsFocusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var isFocus = (bool)e.NewValue;
            var passwordBox = d as Windows.UI.Xaml.Controls.PasswordBox;

            if (passwordBox == null)
                return;

            
        }
    }
}