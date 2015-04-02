using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace ITJakub.MobileApps.Client.Books.View.Control
{
    public class BindableRichTextBlock : ContentControl
    {
        private RichTextBlock RichTextBlock
        {
            get { return (RichTextBlock) Content; }
        }

        public string XamlBlocks
        {
            get { return (string) GetValue(XamlBlocksProperty); }
            set { SetValue(XamlBlocksProperty, value); }
        }

        public int SelectionStart
        {
            get { return (int)GetValue(SelectionStartProperty); }
            set { SetValue(SelectionStartProperty, value); }
        }

        public int SelectionLength
        {
            get { return (int)GetValue(SelectionLengthProperty); }
            set { SetValue(SelectionLengthProperty, value); }
        }

        public static readonly DependencyProperty XamlBlocksProperty = DependencyProperty.Register("XamlBlocks", typeof (string),
            typeof (BindableRichTextBlock), new PropertyMetadata(string.Empty, XamlBlocksChanged));

        public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register(
            "SelectionStart", typeof (int), typeof (BindableRichTextBlock),
            new PropertyMetadata(0, SelectionStartChanged));

        public static readonly DependencyProperty SelectionLengthProperty =
            DependencyProperty.Register("SelectionLength", typeof (int), typeof (BindableRichTextBlock),
                new PropertyMetadata(0, SelectionLengthChanged));

        private static void SelectionLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richTextBlock = d as BindableRichTextBlock;
            if (richTextBlock == null || richTextBlock.RichTextBlock == null)
                return;

            var newSelectionLength = (int)e.NewValue;
            var startTextPointer = richTextBlock.RichTextBlock.ContentStart.GetPositionAtOffset(richTextBlock.SelectionStart, LogicalDirection.Forward);
            var endTextPointer = startTextPointer.GetPositionAtOffset(newSelectionLength,LogicalDirection.Forward);

            richTextBlock.RichTextBlock.Focus(FocusState.Programmatic);
            richTextBlock.RichTextBlock.Select(startTextPointer, endTextPointer);
        }

        private static void SelectionStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richTextBlock = d as BindableRichTextBlock;
            if (richTextBlock == null || richTextBlock.RichTextBlock == null)
                return;

            var newSelectionStart = (int) e.NewValue;
            
            var startTextPointer = richTextBlock.RichTextBlock.ContentStart.GetPositionAtOffset(newSelectionStart, LogicalDirection.Forward);
            var endTextPointer = startTextPointer.GetPositionAtOffset(richTextBlock.SelectionLength, LogicalDirection.Forward);

            richTextBlock.RichTextBlock.Focus(FocusState.Programmatic);
            richTextBlock.RichTextBlock.Select(startTextPointer, endTextPointer);
        }

        private static void XamlBlocksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richTextBlock = d as BindableRichTextBlock;
            if (richTextBlock == null)
                return;

            var newText = (string) e.NewValue;
            
            var text = string.Format("<RichTextBlock xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>{0}</RichTextBlock>", newText);
            object blocksObj;

            try
            {
                blocksObj = XamlReader.Load(text);
            }
            catch (XamlParseException)
            {
                blocksObj = XamlReader.Load("<RichTextBlock xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'></RichTextBlock>");
            }
            richTextBlock.Content = blocksObj;

            richTextBlock.RichTextBlock.IsTextSelectionEnabled = true;
            richTextBlock.RichTextBlock.SelectionHighlightColor = new SolidColorBrush(Colors.Lime);
            richTextBlock.RichTextBlock.Focus(FocusState.Programmatic);
            richTextBlock.RichTextBlock.SelectAll();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {

        }
    }
}
