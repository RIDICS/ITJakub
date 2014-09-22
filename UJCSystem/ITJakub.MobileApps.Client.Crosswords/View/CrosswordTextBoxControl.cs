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
            // Move cursor to correct position
            CursorMargin = new Thickness((Height - 2)*SelectionStart - 1, 0, 0, 0);

            // Select character (Overwrite mode simulation)
            SelectionLength = Text.Length == 0 ? 0 : 1;
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);

            // Disable switching from Overwrite mode to Insert mode
            if (e.Key == VirtualKey.Insert)
            {
                e.Handled = true;
            }

            // Move cursor on key left press
            if (e.Key == VirtualKey.Left && SelectionStart != 0)
            {
                SelectionStart--;
            }
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            var tapPosition = e.GetPosition(this);
            var cursorPosition = tapPosition.X / (Height - 2);
            SelectionStart = (int) cursorPosition;
            e.Handled = true;
        }

        public Thickness CursorMargin
        {
            get { return (Thickness) GetValue(CursorMarginProperty); }
            private set { SetValue(CursorMarginProperty, value); }
        }

        public static readonly DependencyProperty CursorMarginProperty = DependencyProperty.Register("CursorMargin",
            typeof(Thickness), typeof(CrosswordTextBoxControl), new PropertyMetadata(new Thickness(0)));
    }
}
