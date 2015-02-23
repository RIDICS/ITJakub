using Windows.UI.Xaml;

namespace ITJakub.MobileApps.Client.MainApp.View.Control
{
    public sealed partial class DownArrowControl
    {
        public DownArrowControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeStrokeThickness", typeof(double), typeof(DownArrowControl), new PropertyMetadata(1.0));
        
        public static readonly DependencyProperty IsBothWayProperty = DependencyProperty.Register("IsBothWay", typeof(bool), typeof(DownArrowControl), new PropertyMetadata(false));

        public double StrokeThickness
        {
            get { return (double) GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public bool IsBothWay
        {
            get { return (bool) GetValue(IsBothWayProperty); }
            set { SetValue(IsBothWayProperty, value); }
        }
    }
}
