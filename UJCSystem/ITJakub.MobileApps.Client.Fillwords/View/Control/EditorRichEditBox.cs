using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using ITJakub.MobileApps.Client.Books.View.Control;
using ITJakub.MobileApps.Client.Fillwords.ViewModel;

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
                return;
            }

            Document.Selection.SetRange(textRange.StartPosition, textRange.EndPosition);

            SelectedText = textRange.Text.Trim();
            if (WordOptionsList != null && WordOptionsList.ContainsKey(textRange.StartPosition))
            {
                var key = textRange.StartPosition;
                
                SelectedOptions = new OptionsViewModel
                {
                    WordPosition = key,
                    CorrectAnswer = SelectedText,
                    List = new ObservableCollection<OptionViewModel>(WordOptionsList[key].List)
                };
            }
            else
            {
                SelectedOptions = new OptionsViewModel
                {
                    WordPosition = textRange.StartPosition,
                    CorrectAnswer = SelectedText,
                    List = new ObservableCollection<OptionViewModel>()
                };
            }

            IsFlyoutOpen = true;
            IsSelectedTextHighlighted = !textRange.CharacterFormat.BackgroundColor.Equals(m_defaultBackgroundColor);
        }
        
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_contentElement = GetTemplateChild("ContentElement") as ScrollViewer;
        }

        public static readonly DependencyProperty FlyoutProperty = DependencyProperty.Register("Flyout", typeof (Flyout),
            typeof (EditorRichEditBox), new PropertyMetadata(null, OnFlyoutChanged));

        public static readonly DependencyProperty WordOptionsListProperty = DependencyProperty.Register("WordOptionsList",
            typeof (Dictionary<int, OptionsViewModel>), typeof (EditorRichEditBox),
            new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedOptionsProperty = DependencyProperty.Register(
            "SelectedOptions", typeof (OptionsViewModel), typeof (EditorRichEditBox),
            new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedTextProperty = DependencyProperty.Register("SelectedText",
            typeof (string), typeof (EditorRichEditBox), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsFlyoutOpenProperty = DependencyProperty.Register("IsFlyoutOpen", typeof (bool), typeof (EditorRichEditBox), new PropertyMetadata(false, IsFlyoutOpenChanged));

        public static readonly DependencyProperty IsEditingEnabledProperty = DependencyProperty.Register("IsEditingEnabled", typeof (bool), typeof (EditorRichEditBox), new PropertyMetadata(false, IsEditingEnabledChanged));

        public static readonly DependencyProperty IsSelectedTextHighlightedProperty = DependencyProperty.Register("IsSelectedTextHighlighted", typeof (bool), typeof (EditorRichEditBox), new PropertyMetadata(false, OnSelectedTextHighlightChanged));
        
        public static readonly DependencyProperty BackgroundColorHighlightProperty = DependencyProperty.Register("BackgroundColorHighlight", typeof (Color), typeof (EditorRichEditBox), new PropertyMetadata(Colors.SpringGreen));

        public Dictionary<int, OptionsViewModel> WordOptionsList
        {
            get { return (Dictionary<int, OptionsViewModel>)GetValue(WordOptionsListProperty); }
            set { SetValue(WordOptionsListProperty, value); }
        }

        public Flyout Flyout
        {
            get { return (Flyout) GetValue(FlyoutProperty); }
            set { SetValue(FlyoutProperty, value); }
        }

        public OptionsViewModel SelectedOptions
        {
            get { return (OptionsViewModel) GetValue(SelectedOptionsProperty); }
            set { SetValue(SelectedOptionsProperty, value); }
        }

        public string SelectedText
        {
            get { return (string) GetValue(SelectedTextProperty); }
            set { SetValue(SelectedTextProperty, value); }
        }

        public bool IsFlyoutOpen
        {
            get { return (bool) GetValue(IsFlyoutOpenProperty); }
            set { SetValue(IsFlyoutOpenProperty, value); }
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

        private static void OnFlyoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editBox = d as EditorRichEditBox;
            var newFlyout = e.NewValue as Flyout;
            if (editBox == null || newFlyout == null)
                return;

            newFlyout.Opening += (sender, o) => editBox.IsFlyoutOpen = true;
            newFlyout.Closed += (sender, o) => editBox.IsFlyoutOpen = false;
        }

        private static void IsFlyoutOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editBox = d as EditorRichEditBox;
            if (editBox == null || editBox.Flyout == null)
                return;
            
            var isOpen = (bool) e.NewValue;
            if (isOpen)
            {
                editBox.Flyout.ShowAt(editBox);
            }
            else
            {
                editBox.Flyout.Hide();
                editBox.Document.Selection.Collapse(true);
            }
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
        }

        protected override void OnDocumentLoad()
        {
            base.OnDocumentLoad();
            m_defaultBackgroundColor = Document.GetDefaultCharacterFormat().BackgroundColor;
        }
    }
}
