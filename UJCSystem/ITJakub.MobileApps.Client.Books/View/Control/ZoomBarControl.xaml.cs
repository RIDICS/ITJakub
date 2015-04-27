using Windows.UI.Xaml;

namespace ITJakub.MobileApps.Client.Books.View.Control
{
    public sealed partial class ZoomBarControl
    {
        public static readonly DependencyProperty CurrentZoomProperty = DependencyProperty.Register("CurrentZoom", typeof (double), typeof (ZoomBarControl), new PropertyMetadata(0));
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof (int), typeof (ZoomBarControl), new PropertyMetadata(-24));
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof (int), typeof (ZoomBarControl), new PropertyMetadata(24));

        public ZoomBarControl()
        {
            InitializeComponent();
        }

        public double CurrentZoom
        {
            get { return (double) GetValue(CurrentZoomProperty); }
            set { SetValue(CurrentZoomProperty, value); }
        }

        public int Minimum
        {
            get { return (int) GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public int Maximum
        {
            get { return (int) GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        private void ZoomOutButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentZoom - 1 >= Minimum)
            {
                CurrentZoom--;
            }
        }

        private void ZoomInButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentZoom + 1 <= Maximum)
            {
                CurrentZoom++;
            }
        }
    }
}
