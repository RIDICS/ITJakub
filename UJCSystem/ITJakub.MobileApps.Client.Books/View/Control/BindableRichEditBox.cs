using System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Books.View.Control
{
    public class BindableRichEditBox : RichEditBox
    {
        private bool m_recoverSelection;
        private int m_recoverSelectionStart;
        private int m_recoverSelectionEnd;

        public BindableRichEditBox()
        {
            m_recoverSelection = false;
            IsReadOnly = true;
        }

        public string DocumentRtf
        {
            get { return (string) GetValue(DocumentRtfProperty); }
            set { SetValue(DocumentRtfProperty, value);}
        }

        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        public static readonly DependencyProperty DocumentRtfProperty = DependencyProperty.Register("DocumentRtf",
            typeof (string), typeof (BindableRichEditBox),
            new PropertyMetadata(string.Empty, DocumentRtfPropertyChanged));

        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof (double),
            typeof (BindableRichEditBox), new PropertyMetadata(1.0, OnZoomChanged));
        
        private static void DocumentRtfPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richEditBox = d as BindableRichEditBox;
            if (richEditBox == null)
                return;

            var oldIsReadonlyState = richEditBox.IsReadOnly;
            richEditBox.IsReadOnly = false;
            richEditBox.Document.GetRange(0, richEditBox.Document.Selection.StoryLength).CharacterFormat.BackgroundColor
                = richEditBox.Document.GetDefaultCharacterFormat().BackgroundColor;
            richEditBox.Document.SetText(TextSetOptions.FormatRtf, richEditBox.DocumentRtf);
            richEditBox.IsReadOnly = oldIsReadonlyState;

            if (richEditBox.m_recoverSelection)
            {
                richEditBox.Document.Selection.StartPosition = richEditBox.m_recoverSelectionStart;
                richEditBox.Document.Selection.EndPosition = richEditBox.m_recoverSelectionEnd;
                richEditBox.m_recoverSelection = false;
            }
            
            OnZoomChanged(richEditBox, null);
            richEditBox.OnDocumentLoad();
        }

        private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richEditBox = d as BindableRichEditBox;
            if (richEditBox == null)
                return;
            
            var defaultFormat = richEditBox.Document.GetDefaultCharacterFormat();
            var textRange = richEditBox.Document.GetRange(0, richEditBox.Document.Selection.StoryLength);

            var isReadOnly = richEditBox.IsReadOnly;
            richEditBox.IsReadOnly = false;
            textRange.CharacterFormat.Size = (float)(defaultFormat.Size * richEditBox.Zoom);
            richEditBox.IsReadOnly = isReadOnly;

            richEditBox.OnZoomChanged();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            m_recoverSelectionStart = Document.Selection.StartPosition;
            m_recoverSelectionEnd = Document.Selection.EndPosition;
            m_recoverSelection = true;

            string text;
            Document.GetText(TextGetOptions.FormatRtf, out text);
            if (!IsReadOnly && text != DocumentRtf)
                DocumentRtf = text;
        }

        protected virtual void OnDocumentLoad()
        {
            if (DocumentLoaded != null)
                DocumentLoaded(this, new EventArgs());
        }

        protected virtual void OnZoomChanged() { }

        public event EventHandler DocumentLoaded;

        public void ResetDocument()
        {
            DocumentRtfPropertyChanged(this, null);
        }
    }
}
