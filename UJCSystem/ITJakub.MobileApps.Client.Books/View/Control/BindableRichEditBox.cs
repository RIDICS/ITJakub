using System.IO;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Books.View.Control
{
    public class BindableRichEditBox : RichEditBox
    {
        public string DocumentRtf
        {
            get { return (string) GetValue(DocumentRtfProperty); }
            set { SetValue(DocumentRtfProperty, value);}
        }

        public Stream SourceStream
        {
            set { SetValue(SourceStreamProperty, value); }
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
        }

        public static readonly DependencyProperty SourceStreamProperty = DependencyProperty.Register("SourceStream",
            typeof (Stream), typeof (BindableRichEditBox), new PropertyMetadata(null, SourceStreamPropertyChanged));

        private static void SourceStreamPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richEditBox = d as BindableRichEditBox;
            if (richEditBox == null)
                return;

            var oldIsReadonlyState = richEditBox.IsReadOnly;
            richEditBox.IsReadOnly = false;
            richEditBox.Document.LoadFromStream(TextSetOptions.FormatRtf, ((Stream)e.NewValue).AsRandomAccessStream());
            richEditBox.IsReadOnly = oldIsReadonlyState;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
        }
    }
}
