using System.Windows.Input;
using Windows.UI.Xaml;

namespace ITJakub.MobileApps.Client.Books.View.Control
{
    public sealed partial class ZoomBarControl
    {
        public static readonly DependencyProperty CurrentZoomProperty = DependencyProperty.Register("CurrentZoom", typeof (double), typeof (ZoomBarControl), new PropertyMetadata(0));
        public static readonly DependencyProperty ZoomInCommandProperty = DependencyProperty.Register("ZoomInCommand", typeof (ICommand), typeof (ZoomBarControl), new PropertyMetadata(null));
        public static readonly DependencyProperty ZoomOutCommandProperty = DependencyProperty.Register("ZoomOutCommand", typeof (ICommand), typeof (ZoomBarControl), new PropertyMetadata(null));
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

        public ICommand ZoomInCommand
        {
            get { return (ICommand) GetValue(ZoomInCommandProperty); }
            set { SetValue(ZoomInCommandProperty, value); }
        }

        public ICommand ZoomOutCommand
        {
            get { return (ICommand) GetValue(ZoomOutCommandProperty); }
            set { SetValue(ZoomOutCommandProperty, value); }
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
    }
}
