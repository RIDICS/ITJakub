using Windows.UI.Core;
using Windows.UI.Xaml;

namespace ITJakub.MobileApps.Client.Books.View.Control
{
    internal sealed partial class ErrorBarView
    {
        public ErrorBarView()
        {
            InitializeComponent();
            Window.Current.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            Width = e.Size.Width;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            ToCollapsedStoryBoard.Begin();
            Window.Current.SizeChanged -= OnSizeChanged;
        }
    }
}
