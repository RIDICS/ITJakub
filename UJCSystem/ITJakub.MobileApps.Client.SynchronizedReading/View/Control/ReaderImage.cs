using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace ITJakub.MobileApps.Client.SynchronizedReading.View.Control
{
    [TemplatePart(Name = "CursorImage", Type = typeof(Image))]
    [TemplatePart(Name = "SourceImage", Type = typeof(Image))]
    public sealed class ReaderImage : Windows.UI.Xaml.Controls.Control
    {
        private const double FinalPointerCorrectionX = 17.0;
        private const double PointerCorrectionX = 8.0;
        private const double PointerCorrectionY = 30.0;

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof (ImageSource), typeof (ReaderImage), new PropertyMetadata(null));
        public static readonly DependencyProperty PointerPositionXProperty = DependencyProperty.Register("PointerPositionX", typeof (double), typeof (ReaderImage), new PropertyMetadata(0.0, OnPointerPositionChanged));
        public static readonly DependencyProperty PointerPositionYProperty = DependencyProperty.Register("PointerPositionY", typeof (double), typeof (ReaderImage), new PropertyMetadata(0.0, OnPointerPositionChanged));
        public static readonly DependencyProperty PointerCalibrationXProperty = DependencyProperty.Register("PointerCalibrationX", typeof(double), typeof(ReaderImage), new PropertyMetadata(0.0));
        public static readonly DependencyProperty PointerCalibrationYProperty = DependencyProperty.Register("PointerCalibrationY", typeof(double), typeof(ReaderImage), new PropertyMetadata(0.0));
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof (Modes), typeof (ReaderImage), new PropertyMetadata(Modes.Reader));

        private Image m_cursorImage;
        private Image m_sourceImage;

        public ReaderImage()
        {
            DefaultStyleKey = typeof(ReaderImage);
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource) GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public double PointerPositionX
        {
            get { return (double) GetValue(PointerPositionXProperty); }
            set { SetValue(PointerPositionXProperty, value); }
        }

        public double PointerPositionY
        {
            get { return (double) GetValue(PointerPositionYProperty); }
            set { SetValue(PointerPositionYProperty, value); }
        }

        public double PointerCalibrationX
        {
            get { return (double)GetValue(PointerCalibrationXProperty); }
            set { SetValue(PointerCalibrationXProperty, value); }
        }

        public double PointerCalibrationY
        {
            get { return (double)GetValue(PointerCalibrationYProperty); }
            set { SetValue(PointerCalibrationYProperty, value); }
        }

        public Modes Mode
        {
            get { return (Modes) GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }


        private static void OnPointerPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var readerImage = d as ReaderImage;
            if (readerImage == null)
                return;

            var left = readerImage.m_sourceImage.ActualWidth * readerImage.PointerPositionX - FinalPointerCorrectionX;
            var top = readerImage.m_sourceImage.ActualHeight * readerImage.PointerPositionY;

            readerImage.m_cursorImage.Margin = new Thickness(left, top, 0, 0);
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);
            if (Mode == Modes.Reader)
                return;

            var point = e.GetCurrentPoint(m_sourceImage).Position;
            var x = (point.X - PointerCorrectionX - PointerCalibrationX) / m_sourceImage.ActualWidth;
            var y = (point.Y - PointerCorrectionY - PointerCalibrationY) / m_sourceImage.ActualHeight;

            var pointerPositionX = x < 0 ? 0 : x;
            var pointerPositionY = y < 0 ? 0 : y;
            if (x > 1)
                pointerPositionX = 1.0;
            if (y > 1)
                pointerPositionY = 1.0;

            PointerPositionX = pointerPositionX;
            PointerPositionY = pointerPositionY;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_cursorImage = GetTemplateChild("CursorImage") as Image;
            m_sourceImage = GetTemplateChild("SourceImage") as Image;
        }

        public enum Modes
        {
            Reader,
            Pointer
        }
    }
}
