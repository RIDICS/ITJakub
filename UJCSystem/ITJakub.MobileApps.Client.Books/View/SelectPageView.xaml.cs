using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ITJakub.MobileApps.Client.Books.View
{
    public sealed partial class SelectPageView
    {
        public SelectPageView()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            BindableRichEditBox.SelectionHighlightColor = new SolidColorBrush(Colors.Lime);
            BindableRichEditBox.Focus(FocusState.Programmatic);
            BindableRichEditBox.Document.Selection.StartPosition = 5;
            BindableRichEditBox.Document.Selection.EndPosition = 10;
        }
    }
}
