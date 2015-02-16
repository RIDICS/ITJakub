using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace ITJakub.MobileApps.Client.SynchronizedReading.View.Control
{
    [TemplatePart(Name = "CursorImage", Type = typeof(Image))]
    [TemplatePart(Name = "SourceImage", Type = typeof(Image))]
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    public sealed class ReaderImage : Windows.UI.Xaml.Controls.Control
    {
        private const double FinalPointerCorrectionX = 17.0;
        private const double PointerCorrectionX = 8.0;
        private const double PointerCorrectionY = 30.0;
        private const double ScrollStep = 20.0;

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof (ImageSource), typeof (ReaderImage), new PropertyMetadata(null));
        public static readonly DependencyProperty PointerPositionXProperty = DependencyProperty.Register("PointerPositionX", typeof (double), typeof (ReaderImage), new PropertyMetadata(0.0, OnPointerPositionChanged));
        public static readonly DependencyProperty PointerPositionYProperty = DependencyProperty.Register("PointerPositionY", typeof (double), typeof (ReaderImage), new PropertyMetadata(0.0, OnPointerPositionChanged));
        public static readonly DependencyProperty PointerCalibrationXProperty = DependencyProperty.Register("PointerCalibrationX", typeof(double), typeof(ReaderImage), new PropertyMetadata(0.0));
        public static readonly DependencyProperty PointerCalibrationYProperty = DependencyProperty.Register("PointerCalibrationY", typeof(double), typeof(ReaderImage), new PropertyMetadata(0.0));
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof (Modes), typeof (ReaderImage), new PropertyMetadata(Modes.Reader));
        public static readonly DependencyProperty IsScrollingEnabledProperty = DependencyProperty.Register("IsScrollingEnabled", typeof(bool), typeof(ReaderImage), new PropertyMetadata(true, OnScrollingEnabledChanged));

        private Image m_cursorImage;
        private Image m_sourceImage;
        private ScrollViewer m_scrollViewer;
        
        public ReaderImage()
        {
            DefaultStyleKey = typeof(ReaderImage);
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource) GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        /// <summary>
        /// Horizontal pointer position on image in range from 0.0 to 1.0.
        /// </summary>
        public double PointerPositionX
        {
            get { return (double) GetValue(PointerPositionXProperty); }
            set { SetValue(PointerPositionXProperty, value); }
        }

        /// <summary>
        /// Vertical pointer position on image in range from 0.0 to 1.0.
        /// </summary>
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

        public bool IsScrollingEnabled
        {
            get { return (bool) GetValue(IsScrollingEnabledProperty); }
            set { SetValue(IsScrollingEnabledProperty, value); }
        }


        private static void OnPointerPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var readerImage = d as ReaderImage;
            if (readerImage == null)
                return;

            // Count correct pointer position
            var scrollViewer = readerImage.m_scrollViewer;
            var sourceImage = readerImage.m_sourceImage;
            var zoom = scrollViewer.ZoomFactor;
            var zoomedWidth = sourceImage.ActualWidth*zoom;
            var zoomedHeight = sourceImage.ActualHeight*zoom;
            var scrollViewerContentWidth = scrollViewer.ActualWidth - scrollViewer.Padding.Left - scrollViewer.Padding.Right;
            var scrollViewerContentHeight = scrollViewer.ActualHeight - scrollViewer.Padding.Top - scrollViewer.Padding.Bottom;
            
            var left = zoomedWidth * readerImage.PointerPositionX - FinalPointerCorrectionX;  // scaled left image position
            var top = zoomedHeight * readerImage.PointerPositionY;  // scaled top image position

            if (scrollViewerContentWidth >= zoomedWidth)
                left += (scrollViewerContentWidth - zoomedWidth)/2;  // image is in the center of the scrollViewer
            else
                left -= scrollViewer.HorizontalOffset;  // image is wider than scrollViewer, so it's only shifted by horizontal offset

            if (scrollViewerContentHeight >= zoomedHeight)
                top += (scrollViewerContentHeight - zoomedHeight)/2;  // image is in the center of the scrollViewer
            else
                top -= scrollViewer.VerticalOffset;  // image is higher than scrollViewer, so it's only shifted by vertical offset

            // Count scrolling values and new pointer position
            bool isScroll = false;
            double scrollHorizontal = scrollViewer.HorizontalOffset;
            double scrollVertical = scrollViewer.VerticalOffset;
            if (left < -FinalPointerCorrectionX)
            {
                isScroll = true;
                scrollHorizontal += left - ScrollStep + FinalPointerCorrectionX;
                left = scrollHorizontal < 0 ? ScrollStep + scrollHorizontal : ScrollStep;
                left -= FinalPointerCorrectionX;
            }
            else if (left > scrollViewerContentWidth - FinalPointerCorrectionX)
            {
                isScroll = true;
                scrollHorizontal += (left - scrollViewerContentWidth + FinalPointerCorrectionX) + ScrollStep;
                left = scrollViewerContentWidth - ScrollStep - FinalPointerCorrectionX;
                if (scrollHorizontal > scrollViewer.ScrollableWidth) left += scrollHorizontal - scrollViewer.ScrollableWidth;
            }
            if (top < 0)
            {
                isScroll = true;
                scrollVertical += top - ScrollStep;
                top = scrollVertical < 0 ? ScrollStep + scrollVertical : ScrollStep;
            }
            else if (top > scrollViewerContentHeight)
            {
                isScroll = true;
                scrollVertical += (top - scrollViewerContentHeight) + ScrollStep;
                top = scrollViewerContentHeight - ScrollStep;
                if (scrollVertical > scrollViewer.ScrollableHeight) top += scrollVertical - scrollViewer.ScrollableHeight;
            }

            if (isScroll && readerImage.IsScrollingEnabled)
            {
                // scroll and count new pointer position
                scrollViewer.ChangeView(scrollHorizontal, scrollVertical, null);
            }
            
            readerImage.m_cursorImage.Margin = new Thickness(left, top, 0, 0);
        }

        

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);
            if (Mode == Modes.Reader)
                return;

            var point2 = e.GetCurrentPoint(this).Position;
            if (point2.X > ActualWidth - 18 || point2.Y > ActualHeight - 18)
                return;
            
            var point = e.GetCurrentPoint(m_sourceImage).Position;
            var zoom = m_scrollViewer.ZoomFactor;
            var x = (point.X - (PointerCorrectionX + PointerCalibrationX)/zoom) / m_sourceImage.ActualWidth;
            var y = (point.Y - (PointerCorrectionY + PointerCalibrationY)/zoom) / m_sourceImage.ActualHeight;

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
            m_scrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
        }

        private static void OnScrollingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var readerImage = d as ReaderImage;
            if (readerImage == null)
                return;
            
            if ((bool) e.NewValue)
            {
                readerImage.m_scrollViewer.VerticalScrollMode = ScrollMode.Enabled;
                readerImage.m_scrollViewer.HorizontalScrollMode = ScrollMode.Enabled;
                readerImage.m_scrollViewer.ZoomMode = ZoomMode.Enabled;
            }
            else
            {
                readerImage.m_scrollViewer.VerticalScrollMode = ScrollMode.Disabled;
                readerImage.m_scrollViewer.HorizontalScrollMode = ScrollMode.Disabled;
                readerImage.m_scrollViewer.ZoomMode = ZoomMode.Disabled;
            }
            
        }

        public enum Modes
        {
            Reader,
            Pointer
        }
    }
}
