using System.Linq;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using ITJakub.MobileApps.Client.Books.View.Control;
using ITJakub.MobileApps.Client.Shared.Control;

namespace ITJakub.MobileApps.Client.Fillwords.View.Control
{
    [TemplatePart(Name = "ContentElement", Type = typeof(ScrollViewer))]
    public class EditorRichEditBox : BindableRichEditBox
    {
        private ScrollViewer m_contentElement;
        private Color m_defaultBackgroundColor;

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
            if (textRange.Length == 0)
            {
                SelectionStart = -1;
                SelectedText = null;
                IsSelectedTextHighlighted = false;
            }
            else
            {
                Document.Selection.SetRange(textRange.StartPosition, textRange.EndPosition);
                SelectedText = textRange.Text.Trim();
                SelectionStart = textRange.StartPosition;
                IsSelectedTextHighlighted = Document.Selection.CharacterFormat.BackgroundColor.Equals(BackgroundColorHighlight);
            }
            
            InvokeCommand(SelectionChangedCommand);
        }

        private void InvokeCommand(ICommand command)
        {
            if (command != null && command.CanExecute(null))
            {
                command.Execute(null);
            }
        }
        
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_contentElement = GetTemplateChild("ContentElement") as ScrollViewer;
        }

        public static readonly DependencyProperty SelectedTextProperty = DependencyProperty.Register("SelectedText",
            typeof (string), typeof (EditorRichEditBox), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register("SelectionStart", typeof(int), typeof(EditorRichEditBox), new PropertyMetadata(0));
        
        public static readonly DependencyProperty IsEditingEnabledProperty = DependencyProperty.Register("IsEditingEnabled", typeof (bool), typeof (EditorRichEditBox), new PropertyMetadata(false, IsEditingEnabledChanged));

        public static readonly DependencyProperty IsSelectedTextHighlightedProperty = DependencyProperty.Register("IsSelectedTextHighlighted", typeof (bool), typeof (EditorRichEditBox), new PropertyMetadata(false, OnSelectedTextHighlightChanged));
        
        public static readonly DependencyProperty BackgroundColorHighlightProperty = DependencyProperty.Register("BackgroundColorHighlight", typeof (Color), typeof (EditorRichEditBox), new PropertyMetadata(Colors.SpringGreen));
        
        public static readonly DependencyProperty SelectionChangedCommandProperty = DependencyProperty.Register("SelectionChangedCommand", typeof (ICommand), typeof (EditorRichEditBox), new PropertyMetadata(null));

        public static readonly DependencyProperty IsResetProperty = DependencyProperty.Register("IsReset", typeof (bool), typeof (EditorRichEditBox), new PropertyMetadata(true, OnIsResetChanged));
        

        public string SelectedText
        {
            get { return (string) GetValue(SelectedTextProperty); }
            set { SetValue(SelectedTextProperty, value); }
        }
        
        public bool IsEditingEnabled
        {
            get { return (bool) GetValue(IsEditingEnabledProperty); }
            set { SetValue(IsEditingEnabledProperty, value); }
        }

        public bool IsSelectedTextHighlighted
        {
            get { return (bool) GetValue(IsSelectedTextHighlightedProperty); }
            set { SetValue(IsSelectedTextHighlightedProperty, value); }
        }

        public Color BackgroundColorHighlight
        {
            get { return (Color) GetValue(BackgroundColorHighlightProperty); }
            set { SetValue(BackgroundColorHighlightProperty, value); }
        }

        public int SelectionStart
        {
            get { return (int) GetValue(SelectionStartProperty); }
            set { SetValue(SelectionStartProperty, value); }
        }

        public ICommand SelectionChangedCommand
        {
            get { return (ICommand) GetValue(SelectionChangedCommandProperty); }
            set { SetValue(SelectionChangedCommandProperty, value); }
        }

        public bool IsReset
        {
            get { return (bool) GetValue(IsResetProperty); }
            set { SetValue(IsResetProperty, value); }
        }

        private static void IsEditingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editBox = d as EditorRichEditBox;
            if (editBox == null)
                return;

            var isEnabled = (bool)e.NewValue;
            editBox.IsReadOnly = !isEnabled;
        }
        
        private static void OnSelectedTextHighlightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editBox = d as EditorRichEditBox;
            if (editBox == null)
                return;

            var isHighlight = (bool) e.NewValue;
            var color = isHighlight ? editBox.BackgroundColorHighlight : editBox.m_defaultBackgroundColor;
            var readOnlyState = editBox.IsReadOnly;
            editBox.IsReadOnly = false;
            editBox.Document.Selection.CharacterFormat.BackgroundColor = color;
            editBox.IsReadOnly = readOnlyState;

            editBox.IsReset = false;
        }
        
        private static void OnIsResetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editBox = d as EditorRichEditBox;
            if (editBox == null)
                return;

            var isReset = (bool) e.NewValue;
            if (isReset)
                editBox.ResetDocument();
        }

        protected override void OnDocumentLoad()
        {
            base.OnDocumentLoad();
            m_defaultBackgroundColor = Document.GetDefaultCharacterFormat().BackgroundColor;
            IsReset = true;
        }
    }
}
