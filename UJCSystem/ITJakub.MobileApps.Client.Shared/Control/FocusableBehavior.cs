using Windows.UI.Xaml;

namespace ITJakub.MobileApps.Client.Shared.Control
{
    public class FocusableBehavior
    {
        public static readonly DependencyProperty IsFocusProperty =
            DependencyProperty.RegisterAttached("IsFocus", typeof(bool),
                typeof(FocusableBehavior), new PropertyMetadata(false, OnIsFocusPropertyChanged));
        
        public static void SetIsFocus(DependencyObject d, bool value)
        {
            d.SetValue(IsFocusProperty, value);
        }

        public static bool GetIsFocus(DependencyObject d)
        {
            return (bool)d.GetValue(IsFocusProperty);
        }

        private static void OnIsFocusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isFocus = (bool) e.NewValue;
            var control = d as Windows.UI.Xaml.Controls.Control;

            if (control == null)
                return;

            if (isFocus)
            {
                control.Focus(FocusState.Programmatic);
                SetIsFocus(control, false);
            }
        }
    }
}