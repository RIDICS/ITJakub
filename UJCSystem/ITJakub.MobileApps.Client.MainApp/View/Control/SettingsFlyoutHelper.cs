using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.MainApp.View.Control
{
    public class SettingsFlyoutHelper
    {
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.RegisterAttached("IsOpen", typeof(bool),
            typeof(SettingsFlyoutHelper), new PropertyMetadata(true, OnIsOpenPropertyChanged));

        public static void SetIsOpen(DependencyObject d, bool value)
        {
            d.SetValue(IsOpenProperty, value);
        }

        public static bool GetIsOpen(DependencyObject d)
        {
            return (bool)d.GetValue(IsOpenProperty);
        }

        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var settingsFlyout = d as SettingsFlyout;
            var isOpen = (bool) e.NewValue;
            if (settingsFlyout == null)
                return;

            if (isOpen)
                settingsFlyout.Show();
            else
                settingsFlyout.Hide();
        }
    }
}
