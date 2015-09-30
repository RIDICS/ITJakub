using System.Linq;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using ITJakub.MobileApps.Client.Shared.Control;

namespace ITJakub.MobileApps.Client.Fillwords2.View.Control
{
    [TemplatePart(Name = "ContentElement", Type = typeof(ScrollViewer))]
    public class EditorRichEditBox : SelectableRichEditBox
    {
        private ScrollViewer m_contentElement;
        
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            if (IsEditingEnabled)
                return;

            var point = e.GetPosition(this);
            // HACK get scaled point for RichEditBox
            point = ScaleHelper.ScalePoint(true, point.X, point.Y + m_contentElement.VerticalOffset);
            
            var shiftedPoint = new Point(point.X - Padding.Left, point.Y - Padding.Top);
            var textRange = Document.GetRangeFromPoint(shiftedPoint, PointOptions.ClientCoordinates);
            
            textRange.Expand(TextRangeUnit.Word);
            while (textRange.Length > 0 && char.IsWhiteSpace(textRange.Text.Last()))
            {
                textRange.MoveEnd(TextRangeUnit.Character, -1);
            }

            Document.Selection.SetRange(textRange.StartPosition, textRange.EndPosition);
        }
        
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_contentElement = GetTemplateChild("ContentElement") as ScrollViewer;
        }
    }
}
