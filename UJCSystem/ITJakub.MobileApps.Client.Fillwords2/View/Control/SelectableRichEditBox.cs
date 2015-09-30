using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml;
using ITJakub.MobileApps.Client.Books.View.Control;

namespace ITJakub.MobileApps.Client.Fillwords2.View.Control
{
    public class SelectableRichEditBox : BindableRichEditBox
    {
        private Color m_defaultBackgroundColor;

        public SelectableRichEditBox()
        {
            SelectionChanged += OnSelectionChanged;
        }
        
        public static readonly DependencyProperty SelectedTextProperty = DependencyProperty.Register("SelectedText", typeof(string), typeof(SelectableRichEditBox), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register("SelectionStart", typeof(int), typeof(SelectableRichEditBox), new PropertyMetadata(0));

        public static readonly DependencyProperty SelectionEndProperty = DependencyProperty.Register("SelectionEnd", typeof(int), typeof(SelectableRichEditBox), new PropertyMetadata(0));

        public static readonly DependencyProperty IsEditingEnabledProperty = DependencyProperty.Register("IsEditingEnabled", typeof(bool), typeof(SelectableRichEditBox), new PropertyMetadata(false, IsEditingEnabledChanged));

        public static readonly DependencyProperty IsSelectedTextHighlightedProperty = DependencyProperty.Register("IsSelectedTextHighlighted", typeof(bool), typeof(SelectableRichEditBox), new PropertyMetadata(false, OnSelectedTextHighlightChanged));

        public static readonly DependencyProperty BackgroundColorHighlightProperty = DependencyProperty.Register("BackgroundColorHighlight", typeof(Color), typeof(SelectableRichEditBox), new PropertyMetadata(Colors.SpringGreen));

        public static readonly DependencyProperty SelectionChangedCommandProperty = DependencyProperty.Register("SelectionChangedCommand", typeof(ICommand), typeof(SelectableRichEditBox), new PropertyMetadata(null));

        public static readonly DependencyProperty IsResetProperty = DependencyProperty.Register("IsReset", typeof(bool), typeof(SelectableRichEditBox), new PropertyMetadata(true, OnIsResetChanged));

        public string SelectedText
        {
            get { return (string)GetValue(SelectedTextProperty); }
            set { SetValue(SelectedTextProperty, value); }
        }

        public bool IsEditingEnabled
        {
            get { return (bool)GetValue(IsEditingEnabledProperty); }
            set { SetValue(IsEditingEnabledProperty, value); }
        }

        public bool IsSelectedTextHighlighted
        {
            get { return (bool)GetValue(IsSelectedTextHighlightedProperty); }
            set { SetValue(IsSelectedTextHighlightedProperty, value); }
        }

        public Color BackgroundColorHighlight
        {
            get { return (Color)GetValue(BackgroundColorHighlightProperty); }
            set { SetValue(BackgroundColorHighlightProperty, value); }
        }

        public int SelectionStart
        {
            get { return (int)GetValue(SelectionStartProperty); }
            set { SetValue(SelectionStartProperty, value); }
        }

        public int SelectionEnd
        {
            get { return (int)GetValue(SelectionEndProperty); }
            set { SetValue(SelectionEndProperty, value); }
        }

        public ICommand SelectionChangedCommand
        {
            get { return (ICommand)GetValue(SelectionChangedCommandProperty); }
            set { SetValue(SelectionChangedCommandProperty, value); }
        }

        public bool IsReset
        {
            get { return (bool)GetValue(IsResetProperty); }
            set { SetValue(IsResetProperty, value); }
        }

        private static void IsEditingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editBox = d as SelectableRichEditBox;
            if (editBox == null)
                return;

            var isEnabled = (bool)e.NewValue;
            editBox.IsReadOnly = !isEnabled;
        }

        private static void OnSelectedTextHighlightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editBox = d as SelectableRichEditBox;
            if (editBox == null || editBox.SelectionStart < 0 || editBox.SelectionEnd < 0)
                return;

            var isHighlight = (bool)e.NewValue;
            var color = isHighlight ? editBox.BackgroundColorHighlight : editBox.m_defaultBackgroundColor;
            var readOnlyState = editBox.IsReadOnly;
            editBox.IsReadOnly = false;
            editBox.Document.Selection.StartPosition = editBox.SelectionStart;
            editBox.Document.Selection.EndPosition = editBox.SelectionEnd;
            editBox.Document.Selection.CharacterFormat.BackgroundColor = color;
            editBox.IsReadOnly = readOnlyState;

            editBox.IsReset = false;
        }

        private static void OnIsResetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editBox = d as SelectableRichEditBox;
            if (editBox == null)
                return;

            var isReset = (bool)e.NewValue;
            if (isReset)
                editBox.ResetDocument();
        }

        protected override void OnDocumentLoad()
        {
            base.OnDocumentLoad();
            m_defaultBackgroundColor = Document.GetDefaultCharacterFormat().BackgroundColor;
            IsReset = true;
        }

        private void OnSelectionChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Document.Selection.Length == 0)
            {
                SelectionStart = -1;
                SelectionEnd = -1;
                SelectedText = null;
                IsSelectedTextHighlighted = false;
            }
            else
            {
                var selection = Document.Selection;
                SelectedText = selection.Text.Trim();
                SelectionStart = selection.StartPosition;
                SelectionEnd = selection.EndPosition;
                IsSelectedTextHighlighted = Document.Selection.CharacterFormat.BackgroundColor.Equals(BackgroundColorHighlight);
            }

            InvokeCommand(SelectionChangedCommand);
        }

        protected void InvokeCommand(ICommand command)
        {
            if (command != null && command.CanExecute(null))
            {
                command.Execute(null);
            }
        }
    }
}