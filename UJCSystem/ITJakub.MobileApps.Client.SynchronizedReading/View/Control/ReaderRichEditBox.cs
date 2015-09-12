using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using ITJakub.MobileApps.Client.Books.View.Control;
using ITJakub.MobileApps.Client.Shared.Control;

namespace ITJakub.MobileApps.Client.SynchronizedReading.View.Control
{
    [TemplatePart(Name = "RichEditBox", Type = typeof(BindableRichEditBox))]
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "OverlapGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "CursorImage", Type = typeof(Image))]
    [TemplatePart(Name = "ScrollOverlapGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "ScrollBarGrid", Type = typeof(Grid))]
    public sealed class ReaderRichEditBox : Windows.UI.Xaml.Controls.Control
    {
        private const double CursorCorrectionLeft = -9.0;
        private const double CursorCorrectionTop = 3.0;
        private const double PointerCorrectionTop = 60.0;
        private const double PointerCorrectionLeft = 20.0;

        private bool m_selectionChangedRespond;
        private BindableRichEditBox m_richEditBox;
        private ScrollViewer m_scrollViewer;
        private Grid m_overlapGrid;
        private Image m_cursorImage;
        private Grid m_scrollOverlapGrid;
        private Grid m_scrollBarGrid;

        public ReaderRichEditBox()
        {
            m_selectionChangedRespond = true;
            DefaultStyleKey = typeof(ReaderRichEditBox);
            SizeChanged += (sender, args) => OnCursorPositionChanged(this, null);
        }

        public static readonly DependencyProperty DocumentRtfProperty = DependencyProperty.Register("DocumentRtf", typeof(string), typeof(ReaderRichEditBox), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register("SelectionStart", typeof(int), typeof(ReaderRichEditBox), new PropertyMetadata(0, OnSelectionPropertyChanged));

        public static readonly DependencyProperty SelectionLengthProperty = DependencyProperty.Register("SelectionLength", typeof(int), typeof(ReaderRichEditBox), new PropertyMetadata(0, OnSelectionPropertyChanged));
        
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof (Modes), typeof (ReaderRichEditBox), new PropertyMetadata(Modes.Reader, OnModeChanged));
        
        public static readonly DependencyProperty SelectionChangedCommandProperty = DependencyProperty.Register("SelectionChangedCommand", typeof (ICommand), typeof (ReaderRichEditBox), new PropertyMetadata(null));
        
        public static readonly DependencyProperty CursorPositionProperty = DependencyProperty.Register("CursorPosition", typeof (int), typeof (ReaderRichEditBox), new PropertyMetadata(0, OnCursorPositionChanged));

        public static readonly DependencyProperty PointerCalibrationXProperty = DependencyProperty.Register("PointerCalibrationX", typeof (double), typeof (ReaderRichEditBox), new PropertyMetadata(0.0));

        public static readonly DependencyProperty PointerCalibrationYProperty = DependencyProperty.Register("PointerCalibrationY", typeof (double), typeof (ReaderRichEditBox), new PropertyMetadata(0.0));
        
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof (double), typeof (ReaderRichEditBox), new PropertyMetadata(1.0, OnZoomChanged));
        
        public string DocumentRtf
        {
            get { return (string) GetValue(DocumentRtfProperty); }
            set { SetValue(DocumentRtfProperty, value); }
        }

        public int SelectionStart
        {
            get { return (int) GetValue(SelectionStartProperty); }
            set { SetValue(SelectionStartProperty, value); }
        }

        public int SelectionLength
        {
            get { return (int) GetValue(SelectionLengthProperty); }
            set { SetValue(SelectionLengthProperty, value); }
        }

        public Modes Mode
        {
            get { return (Modes) GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public ICommand SelectionChangedCommand
        {
            get { return (ICommand) GetValue(SelectionChangedCommandProperty); }
            set { SetValue(SelectionChangedCommandProperty, value); }
        }

        public int CursorPosition
        {
            get { return (int) GetValue(CursorPositionProperty); }
            set { SetValue(CursorPositionProperty, value); }
        }

        public double PointerCalibrationX
        {
            get { return (double) GetValue(PointerCalibrationXProperty); }
            set { SetValue(PointerCalibrationXProperty, value); }
        }

        public double PointerCalibrationY
        {
            get { return (double) GetValue(PointerCalibrationYProperty); }
            set { SetValue(PointerCalibrationYProperty, value); }
        }

        public double Zoom
        {
            get { return (double) GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        private static void OnSelectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var readerRichEditBox = d as ReaderRichEditBox;
            if (readerRichEditBox == null)
                return;

            var richEditBox = readerRichEditBox.m_richEditBox;
            if (readerRichEditBox.m_selectionChangedRespond)
                richEditBox.Document.Selection.SetRange(readerRichEditBox.SelectionStart, readerRichEditBox.SelectionStart + readerRichEditBox.SelectionLength);

            if (richEditBox.FocusState == FocusState.Unfocused)
                richEditBox.Focus(FocusState.Programmatic);

            readerRichEditBox.ScrollToSelectedText();
        }

        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            m_selectionChangedRespond = false;
            SelectionStart = m_richEditBox.Document.Selection.StartPosition;
            SelectionLength = m_richEditBox.Document.Selection.EndPosition - m_richEditBox.Document.Selection.StartPosition;
            m_selectionChangedRespond = true;
            if (Mode != Modes.Reader && SelectionChangedCommand != null && SelectionChangedCommand.CanExecute(null))
            {
                SelectionChangedCommand.Execute(null);
            }
        }

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var readerRichEditBox = d as ReaderRichEditBox;
            if (readerRichEditBox == null || readerRichEditBox.m_overlapGrid == null)
                return;

            switch (readerRichEditBox.Mode)
            {
                case Modes.Selector:
                    readerRichEditBox.m_overlapGrid.Visibility = Visibility.Collapsed;
                    readerRichEditBox.m_scrollOverlapGrid.Visibility = Visibility.Collapsed;
                    readerRichEditBox.m_scrollBarGrid.Visibility = Visibility.Collapsed;
                    break;
                case Modes.Pointer:
                    readerRichEditBox.m_overlapGrid.Visibility = Visibility.Visible;
                    readerRichEditBox.m_scrollOverlapGrid.Visibility = Visibility.Visible;
                    readerRichEditBox.m_scrollBarGrid.Visibility = Visibility.Visible;
                    break;
                default:
                    readerRichEditBox.m_overlapGrid.Visibility = Visibility.Visible;
                    readerRichEditBox.m_scrollOverlapGrid.Visibility = Visibility.Collapsed;
                    readerRichEditBox.m_scrollBarGrid.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private static void OnCursorPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var readerRichEditBox = d as ReaderRichEditBox;
            if (readerRichEditBox == null || readerRichEditBox.m_overlapGrid == null)
                return;

            var newPosition = readerRichEditBox.CursorPosition;
            var textRange = readerRichEditBox.m_richEditBox.Document.GetRange(newPosition, newPosition + 1);
            
            Rect rect;
            int hit;
            textRange.GetRect(PointOptions.ClientCoordinates, out rect, out hit);
            var x = (rect.Left + rect.Right)/2;
            var y = rect.Top;
            var bottom = rect.Bottom;

            // HACK get scaled point coordinates for RichEditBox
            var point = ScaleHelper.ScalePoint(false, x, y);
            bottom = ScaleHelper.ScaleValue(false, bottom);

            var newMargin = new Thickness(point.X + CursorCorrectionLeft, bottom + CursorCorrectionTop, 0, 0);
            readerRichEditBox.m_cursorImage.Margin = newMargin;

            var scrollOffset = readerRichEditBox.m_scrollViewer.VerticalOffset;
            if (point.Y < scrollOffset)
            {
                readerRichEditBox.m_scrollViewer.ChangeView(null, point.Y - ScaleHelper.ScaleValue(false, 20), null);
            }
            else if (bottom > scrollOffset + readerRichEditBox.ActualHeight - 60)
            {
                readerRichEditBox.m_scrollViewer.ChangeView(null, point.Y - readerRichEditBox.ActualHeight + 90, null);
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_richEditBox = GetTemplateChild("RichEditBox") as BindableRichEditBox;
            m_scrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
            m_overlapGrid = GetTemplateChild("OverlapGrid") as Grid;
            m_cursorImage = GetTemplateChild("CursorImage") as Image;
            m_scrollOverlapGrid = GetTemplateChild("ScrollOverlapGrid") as Grid;
            m_scrollBarGrid = GetTemplateChild("ScrollBarGrid") as Grid;

            if (m_richEditBox != null)
            {
                m_richEditBox.SelectionChanged += OnSelectionChanged;
            }

            OnModeChanged(this, null);
        }
        
        private void ScrollToSelectedText()
        {
            Rect rectangle;
            int hit;

            m_richEditBox.Document.Selection.GetRect(PointOptions.ClientCoordinates, out rectangle, out hit);
            
            // HACK get scaled values using ScaleValue method
            var top = ScaleHelper.ScaleValue(false, rectangle.Top + 3);
            var bottom = ScaleHelper.ScaleValue(false, rectangle.Bottom + 3);

            var textHeight = bottom - top;
            var boxHeight = ActualHeight;
            var scrollOffset = m_scrollViewer.VerticalOffset;

            if (top < scrollOffset || bottom > scrollOffset + boxHeight)
            {
                var scrollTo = textHeight < boxHeight - 40 ? top - 20 : top;
                m_scrollViewer.ChangeView(null, scrollTo, null);
            }
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);
            if (Mode != Modes.Pointer)
                return;

            var point = e.GetCurrentPoint(m_richEditBox).Position;

            point.X -= PointerCorrectionLeft - PointerCalibrationX;
            point.Y -= PointerCorrectionTop + PointerCalibrationY;

            // HACK get unscaled point coordinates for RichEditBox
            point = ScaleHelper.ScalePoint(true, point.X, point.Y);
            
            var textRange = m_richEditBox.Document.GetRangeFromPoint(point, PointOptions.ClientCoordinates);
            CursorPosition = textRange.StartPosition;
        }
        
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            base.OnPointerWheelChanged(e);
            var properties = e.GetCurrentPoint(this).Properties;
            var delta = properties.MouseWheelDelta;
            m_scrollViewer.ChangeView(null, m_scrollViewer.VerticalOffset - delta, null);
        }
        
        private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var readerRichEditBox = d as ReaderRichEditBox;
            if (readerRichEditBox == null)
                return;

            var richEditBox = readerRichEditBox.m_richEditBox;
            richEditBox.Zoom = readerRichEditBox.Zoom;
            
            // TODO improve logic
            OnCursorPositionChanged(d, null);
        }
        
        public enum Modes
        {
            Reader, Pointer, Selector
        }
    }
}
