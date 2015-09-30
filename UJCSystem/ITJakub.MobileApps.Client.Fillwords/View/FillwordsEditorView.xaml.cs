using Windows.UI.Xaml;

namespace ITJakub.MobileApps.Client.Fillwords.View
{
    public sealed partial class FillwordsEditorView
    {
        public FillwordsEditorView()
        {
            InitializeComponent();
        }

        private void PasteButton_OnClick(object sender, RoutedEventArgs e)
        {
            EditorRichEditBox.Document.Selection.Paste(0);
        }
    }
}
