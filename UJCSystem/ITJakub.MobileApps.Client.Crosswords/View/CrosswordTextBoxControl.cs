using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace ITJakub.MobileApps.Client.Crosswords.View
{
    public class CrosswordTextBoxControl : TextBox
    {
        public CrosswordTextBoxControl()
        {
            //DefaultStyleKey = typeof(CrosswordTextBoxControl);
            SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            CursorMargin = new Thickness(20 * SelectionStart, 0, 0, 0);
        }

        public Thickness CursorMargin
        {
            get { return (Thickness)GetValue(CursorMarginProperty); }
            set { SetValue(CursorMarginProperty, value); }
        }

        public static readonly DependencyProperty CursorMarginProperty = DependencyProperty.Register("CursorMargin",
            typeof(Thickness), typeof(CrosswordTextBoxControl), new PropertyMetadata(new Thickness(0)));
    }
}
