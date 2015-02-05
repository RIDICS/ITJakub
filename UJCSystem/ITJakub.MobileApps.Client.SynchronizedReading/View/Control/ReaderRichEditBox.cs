using System.Windows.Input;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using ITJakub.MobileApps.Client.Books.View.Control;

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
        private const double PointerCorrectionTop = 40.0;
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

            DocumentRtf = //TODO only for test
                "{\\rtf1\\fbidis\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 Segoe UI;}{\\f1\\fnil\\fcharset238 Segoe UI;}}\r\n{\\colortbl ;\\red0\\green0\\blue0;}\r\n{\\*\\generator Riched20 6.3.9600}\\viewkind4\\uc1 \r\n\\pard\\ltrpar\\tx720\\cf1\\f0\\fs22 Tuto se po\\f1\\'e8\\'edn\\'e1 p\\'f8edmluva dosp\\'ecl\\'e9ho mu\\'9ee Gvidona z Kolumny mez\\'e1nsk\\'e9 na Kroniku troj\\'e1nsk\\'fa\\par\r\n\\par\r\nKterak\\'9ekoli d\\'e1vn\\'e9 v\\'ecci nov\\'fdmi druhdy zapadaj\\'ed, v\\'9aak n\\'ecker\\'e9 v\\'ecci d\\'e1vn\\'e9 d\\'e1vno pominuly js\\'fa, kter\\'e9\\'9eto sv\\'fa velikost\\'ed duostojny js\\'fa \\'9eiv\\'facie pam\\'ecti, tak\\'9ee ani jich vetchost slep\\'fdm uhryz\\'e1n\\'edm muo\\'9e zahladiti, ani minul\\'e9ho \\'e8asu vetch\\'e1 l\\'e9ta usnul\\'fa ml\\'e8edlivost\\'ed zav\\'f8ieti. Neb trvaj\\'ed v nich d\\'f8ieve d\\'e1l\\'fdch v\\'ecc\\'ed velebnosti \\'fastavn\\'e1 vzpom\\'ednanie, kdy\\'9eto od p\\'f8edkuov ku potomn\\'edm \\'f8e\\'e8 b\\'fdv\\'e1 v\\'ecrn\\'fdm p\\'edsmem pops\\'e1na, a d\\'e1vn\\'fdch sklada\\'e8uov v\\'ecrn\\'e1 pops\\'e1nie, kter\\'e1\\'9eto sv\\'fdm zachov\\'e1n\\'edm minul\\'e9 jako\\'9eto p\\'f8\\'edtomn\\'e9 miern\\'ecjie okazuj\\'ed, a mu\\'9euov state\\'e8n\\'fdch, kter\\'e9\\'9eto dl\\'fah\\'fd v\\'eck sv\\'ecta d\\'e1vno skrze smrt pohltil, bedliv\\'fdm \\'e8\\'edtan\\'edm knih \\'9eiv\\'e9 obrazy na\\'9aim duch\\'f3m oznamuje.\\par\r\n\\par\r\nProto\\'9e troj\\'e1nsk\\'e9ho m\\'ecsta zru\\'9aenie nenie hodn\\'e9, by kter\\'e9ho dl\\'fah\\'e9ho \\'e8asu vetchost\\'ed bylo zahlazeno, jedno aby ustavi\\'e8n\\'fdm zpom\\'ednan\\'edm kvetlo na mysli lidsk\\'e9, mnoh\\'fdch p\\'edsa\\'f8uov ruka v\\'ecrn\\'fdm p\\'edsmem popsala jest. Mnoz\\'ed tak\\'e9 sklada\\'e8i t\\'e9to p\\'f8\\'edhody st\\'e1l\\'fa pravdu oby\\'e8ejem hercov\\'fdm v rozli\\'e8n\\'e1 podobenstvie s\\'fa p\\'f8etva\\'f8ovali, tak\\'9ee s\\'fa shled\\'e1ni v nepravd\\'ec, ale vymy\\'9alen\\'e9 b\\'e1sn\\'ec sepsav\\'9ae. Mezi nimi\\'9eto za sv\\'fdch dn\\'f3v p\\'f8evelik\\'e9 vz\\'e1cnosti Om\\'e9rus, skladatel \\'f8eck\\'fd, ty d\\'e1l\\'e9 v\\'ecci, p\\'fah\\'fa a prost\\'fa pravdu, v chytr\\'e9 rozpr\\'e1vky prom\\'ecnil jest, zam\\'fd\\'9aleje mnoh\\'e9 v\\'ecci, kter\\'e9\\'9e js\\'fa se ned\\'e1ly, a kter\\'e9\\'9e js\\'fa se d\\'e1ly, jinak p\\'f8etva\\'f8uje. Neb jest pravil, \\'9ee by bohov\\'e9, jim\\'9e se jest klan\\'ecla d\\'e1vn\\'e1 zpohanilost \\'f8eck\\'e1, oni troj\\'e1nsk\\'e9ho m\\'ecsta dob\\'fdvali, a v lidsk\\'e9 tv\\'e1rnosti bojuj\\'edce, mnoz\\'ed z nich se\\'9ali. Jeho\\'9eto bludu mnoz\\'ed chytrci v\\'9aete\\'e8n\\'ec n\\'e1sleduj\\'edce, dali s\\'fa vtipn\\'fdm srozum\\'ecti, \\'9ee Om\\'e9rus byl sklada\\'e8 b\\'e1sniv\\'fdch z\\'e1mysluov; a oni v tom jeho n\\'e1sledovn\\'edci byli, p\\'ed\\'9a\\'edce knihy rozli\\'e8n\\'e9. Proto\\'9e Ovidius sulmonensk\\'fd rukotr\\'9en\\'fdm skl\\'e1dan\\'edm ve mnoh\\'fdch knih\\'e1ch ob\\'e9 jest zuosnoval a mnoho z\\'e1mysluov k z\\'e1mysl\\'f3m p\\'f8i\\'e8inil, a mezi tiem n\\'eckde druhdy pravdy neop\\'fa\\'9at\\'ecje. Tak\\'e9[b] Virgilius v knih\\'e1ch sv\\'fdch Eneidorum[c] p\\'ed\\'9ae, jak\\'9ekoli v\\'9edy, kdy\\'9e se skutkuov troj\\'e1nsk\\'fdch dotekl, sv\\'ectle pravdu. Av\\'9aak od druh\\'fdch z\\'e1mysluov Om\\'e9rov\\'fdch necht\\'ecl sv\\'e9 ruky zdr\\'9eeti.\\par\r\n\\par\r\nNe\\'9e aby v\\'ecrn\\'fdch p\\'edsa\\'f8uov prav\\'e1 p\\'edsma o t\\'ecch d\\'e1l\\'fdch v\\'eccech mezi lidmi na z\\'e1pad slunce p\\'f8eb\\'fdvaj\\'edc\\'edm v bud\\'fac\\'edch \\'e8asiech v\\'ec\\'e8n\\'ec zuostala k \\'fa\\'9eitku, zvl\\'e1\\'9at\\'ec t\\'ecm, je\\'9ato kroniky rozli\\'e8n\\'e9 \\'e8\\'edtaj\\'ed, aby um\\'ecli rozeznati pravdu od k\\'f8ivdy, a ku prosp\\'ecchu udatenstvie lidu rytie\\'f8sk\\'e9ho, ty v\\'ecci, kter\\'e9\\'9e skrze Dita \\'f8e\\'e8sk\\'e9ho a Dana troj\\'e1nsk\\'e9ho js\\'fa pops\\'e1ny, kte\\'f8\\'ed\\'9eto v ty \\'e8asy byli p\\'f8\\'edtomni u vojsk\\'e1ch, a co js\\'fa o\\'e8it\\'ec spat\\'f8ili, to js\\'fa v\\'ecrn\\'ec popsali, a skrze m\\'ec, s\\'fadci Gvidona z Kolumny mez\\'e1nsk\\'e9, v tyto knihy js\\'fa seps\\'e1ny, jako\\'9e to v jich\\f0\\par\r\n\r\n\\pard\\ltrpar\\tx720\\par\r\n}\r\n\u0000";
        }

        public static readonly DependencyProperty DocumentRtfProperty = DependencyProperty.Register("DocumentRtf", typeof(string), typeof(ReaderRichEditBox), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register("SelectionStart", typeof(int), typeof(ReaderRichEditBox), new PropertyMetadata(0, OnSelectionPropertyChanged));

        public static readonly DependencyProperty SelectionLengthProperty = DependencyProperty.Register("SelectionLength", typeof(int), typeof(ReaderRichEditBox), new PropertyMetadata(0, OnSelectionPropertyChanged));
        
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof (Mode), typeof (ReaderRichEditBox), new PropertyMetadata(Mode.Reader, OnModeChanged));
        
        public static readonly DependencyProperty SelectionChangedCommandProperty = DependencyProperty.Register("SelectionChangedCommand", typeof (ICommand), typeof (ReaderRichEditBox), new PropertyMetadata(null));
        
        public static readonly DependencyProperty CursorPositionProperty = DependencyProperty.Register("CursorPosition", typeof (int), typeof (ReaderRichEditBox), new PropertyMetadata(0, OnCursorPositionChanged));

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

        public Mode Mode
        {
            get { return (Mode) GetValue(ModeProperty); }
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
            if (Mode != Mode.Reader && SelectionChangedCommand != null && SelectionChangedCommand.CanExecute(null))
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
                case Mode.Selector:
                    readerRichEditBox.m_overlapGrid.Visibility = Visibility.Collapsed;
                    readerRichEditBox.m_scrollOverlapGrid.Visibility = Visibility.Collapsed;
                    readerRichEditBox.m_scrollBarGrid.Visibility = Visibility.Collapsed;
                    break;
                case Mode.Pointer:
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
            var point = ScalePoint(false, x, y);
            bottom = ScaleValue(false, bottom);

            var newMargin = new Thickness(point.X + CursorCorrectionLeft, bottom + CursorCorrectionTop, 0, 0);
            readerRichEditBox.m_cursorImage.Margin = newMargin;

            var scrollOffset = readerRichEditBox.m_scrollViewer.VerticalOffset;
            if (point.Y < scrollOffset)
            {
                readerRichEditBox.m_scrollViewer.ChangeView(null, point.Y - ScaleValue(false, 20), null);
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
                m_richEditBox.SelectionChanged += OnSelectionChanged;

            OnModeChanged(this, null);
        }
        
        private void ScrollToSelectedText()
        {
            Rect rectangle;
            int hit;

            m_richEditBox.Document.Selection.GetRect(PointOptions.ClientCoordinates, out rectangle, out hit);
            
            // HACK get scaled values using ScaleValue method
            var top = ScaleValue(false, rectangle.Top + 3);
            var bottom = ScaleValue(false, rectangle.Bottom + 3);

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
            if (Mode != Mode.Pointer)
                return;

            var point = e.GetCurrentPoint(m_richEditBox).Position;

            point.X -= PointerCorrectionLeft;
            point.Y -= PointerCorrectionTop;

            // HACK get unscaled point coordinates for RichEditBox
            point = ScalePoint(true, point.X, point.Y);
            
            var textRange = m_richEditBox.Document.GetRangeFromPoint(point, PointOptions.ClientCoordinates);
            CursorPosition = textRange.StartPosition;
        }

        private static Point ScalePoint(bool inverse, double x, double y)
        {
            var resolutionScale = DisplayInformation.GetForCurrentView().ResolutionScale;
            if (resolutionScale == ResolutionScale.Scale100Percent)
                return new Point(x, y);
            
            var scale = (double) resolutionScale / 100.0;

            return inverse ? new Point(x / scale, y / scale) : new Point(x * scale, y * scale);
        }

        private static double ScaleValue(bool inverse, double value)
        {
            var resolutionScale = DisplayInformation.GetForCurrentView().ResolutionScale;
            if (resolutionScale == ResolutionScale.Scale100Percent)
                return value;

            var scale = (double)resolutionScale / 100.0;

            return inverse ? value/ scale : value * scale;
        }

        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            base.OnPointerWheelChanged(e);
            var properties = e.GetCurrentPoint(this).Properties;
            var delta = properties.MouseWheelDelta;
            m_scrollViewer.ChangeView(null, m_scrollViewer.VerticalOffset - delta, null);
        }
    }

    public enum Mode
    {
        Reader, Pointer, Selector
    }
}
