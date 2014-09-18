using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235
using Windows.UI.Xaml.Input;

namespace ITJakub.MobileApps.Client.Crosswords.View
{
    public class CrosswordTextBoxControl : TextBox
    {
        public CrosswordTextBoxControl()
        {
            DefaultStyleKey = typeof(CrosswordTextBoxControl);
            SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            CursorMargin = new Thickness((Height-2) * SelectionStart - 1, 0, 0, 0);
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == VirtualKey.Insert)
            {
                e.Handled = true;
                return;
            }
        }

        public Thickness CursorMargin
        {
            get { return (Thickness) GetValue(CursorMarginProperty); }
            set { SetValue(CursorMarginProperty, value); }
        }

        public static readonly DependencyProperty CursorMarginProperty = DependencyProperty.Register("CursorMargin",
            typeof(Thickness), typeof(CrosswordTextBoxControl), new PropertyMetadata(new Thickness(0)));
    }
}
