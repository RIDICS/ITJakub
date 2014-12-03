using System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Books.View.Control
{
    public class BindableRichEditBox : RichEditBox
    {
        public BindableRichEditBox()
        {
            IsReadOnly = true;
        }

        public string DocumentRtf
        {
            get { return (string) GetValue(DocumentRtfProperty); }
            set { SetValue(DocumentRtfProperty, value);}
        }

        public static readonly DependencyProperty DocumentRtfProperty = DependencyProperty.Register("DocumentRtf",
            typeof (string), typeof (BindableRichEditBox),
            new PropertyMetadata(string.Empty, DocumentRtfPropertyChanged));

        private static void DocumentRtfPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richEditBox = d as BindableRichEditBox;
            if (richEditBox == null)
                return;

            var oldIsReadonlyState = richEditBox.IsReadOnly;
            richEditBox.IsReadOnly = false;
            richEditBox.Document.SetText(TextSetOptions.FormatRtf, e.NewValue.ToString());
            richEditBox.IsReadOnly = oldIsReadonlyState;
            richEditBox.OnDocumentLoad();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
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

        public event EventHandler DocumentLoaded;
    }
}
