using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ITJakub.MobileApps.Client.Crosswords.View
{
    [TemplatePart(Name = "ShowKeyboardButton", Type = typeof(Button))]
    [TemplatePart(Name = "ContentElement", Type = typeof(ItemsControl))]
    public class CrosswordTextBoxControl : TextBox
    {
        private TextBoxState m_textBoxState;
        private ItemsControl m_contentElement;
        private bool m_isActualItemWidthSet;

        public CrosswordTextBoxControl()
        {
            DefaultStyleKey = typeof(CrosswordTextBoxControl);
            SelectionChanged += OnSelectionChanged;
            TextChanged += OnTextChanged;
        }

        private void UpdateActualItemWidth()
        {
            var itemPanelRoot = m_contentElement.ItemsPanelRoot;
            if (itemPanelRoot != null)
            {
                var childItem = itemPanelRoot.Children.FirstOrDefault() as FrameworkElement;
                if (childItem != null)
                {
                    ActualItemWidth = childItem.ActualWidth;
                    m_isActualItemWidthSet = true;
                }
            }
        }

        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!m_isActualItemWidthSet)
                UpdateActualItemWidth();

            // Move cursor to correct position
            if (SelectionStart == 0)
            {
                CursorMargin = new Thickness(-1, 0, 0, 0);
            }
            else
            {
                var width = ActualItemWidth < 1 ? ActualHeight - 2 : ActualItemWidth;
                CursorMargin = new Thickness(width*SelectionStart - 1, 0, 0, 0);
            }

            // Select character (Overwrite mode simulation)
            SelectionLength = Text.Length == 0 ? 0 : 1;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var uppercaseText = Text.ToUpper();
            if (uppercaseText == Text)
                return;

            var selectionStart = SelectionStart;
            Text = uppercaseText;
            SelectionStart = selectionStart;
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);

            // Disable switching from Overwrite mode to Insert mode
            if (e.Key == VirtualKey.Insert)
            {
                e.Handled = true;
            }

            // Move cursor on key left press
            if (e.Key == VirtualKey.Left && SelectionStart != 0)
            {
                SelectionStart--;
            }
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            var tapPosition = e.GetPosition(this);
            var cursorPosition = tapPosition.X / (Height - 2);

            if (cursorPosition < MaxLength)
                SelectionStart = (int)cursorPosition;
            
            e.Handled = true;
        }

        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.Key == VirtualKey.Enter)
            {
                FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
            }
        }

        private void InsertLetter(char letter)
        {
            var selectionStart = SelectionStart;
            var newText = new StringBuilder(Text);
            if (selectionStart < newText.Length)
            {
                newText[selectionStart] = letter;
            }
            else if (selectionStart < MaxLength)
            {
                newText.Append(letter);
            }
            else
            {
                return;
            }
            
            Text = newText.ToString();
            SelectionStart = selectionStart + 1;
        }

        public Thickness CursorMargin
        {
            get { return (Thickness) GetValue(CursorMarginProperty); }
            private set { SetValue(CursorMarginProperty, value); }
        }

        public string KeyboardLetters
        {
            get { return (string) GetValue(KeyboardLettersProperty); }
            set { SetValue(KeyboardLettersProperty, value); }
        }
        
        public ObservableCollection<ButtonViewModel> KeyboardViewModel
        {
            get { return (ObservableCollection<ButtonViewModel>) GetValue(KeyboardViewModelProperty); }
        }

        public double ActualItemWidth
        {
            get { return (double)GetValue(ActualItemWidthProperty); }
            set { SetValue(ActualItemWidthProperty, value); }
        }

        public int Shift
        {
            get { return (int) GetValue(ShiftProperty); }
            set { SetValue(ShiftProperty, value); }
        }

        public static readonly DependencyProperty CursorMarginProperty = DependencyProperty.Register("CursorMargin",
            typeof(Thickness), typeof(CrosswordTextBoxControl), new PropertyMetadata(new Thickness(-1,0,0,0)));

        public static readonly DependencyProperty KeyboardLettersProperty = DependencyProperty.Register("KeyboardLetters",
            typeof (string), typeof (CrosswordTextBoxControl), new PropertyMetadata(string.Empty, OnKeyboardLettersChanged));

        public static readonly DependencyProperty KeyboardViewModelProperty =
            DependencyProperty.Register("KeyboardViewModel", typeof (ObservableCollection<ButtonViewModel>),
                typeof (CrosswordTextBoxControl), new PropertyMetadata(new ObservableCollection<ButtonViewModel>()));

        public static readonly DependencyProperty ActualItemWidthProperty =
            DependencyProperty.Register("ActualItemWidth", typeof (double),
                typeof(CrosswordTextBoxControl), new PropertyMetadata(0.0));

        public static readonly DependencyProperty ShiftProperty = DependencyProperty.Register("Shift", typeof (int),
            typeof (CrosswordTextBoxControl), new PropertyMetadata(0, OnShiftChanged));


        private static void OnShiftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as CrosswordTextBoxControl;
            if (textBox == null)
                return;

            var width = textBox.ActualItemWidth;
            if (width < 1)
                width = textBox.ActualHeight - 2;

            textBox.Margin = new Thickness(width*textBox.Shift, 0, 0, 0);
        }
        
        private static void OnKeyboardLettersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as CrosswordTextBoxControl;
            var text = e.NewValue as string;
            if (textBox == null || text == null)
                return;

            textBox.KeyboardViewModel.Clear();
            foreach (var letter in text)
            {
                textBox.KeyboardViewModel.Add(new ButtonViewModel
                {
                    Label = letter
                });
            }
        }


        // Section for handling Visual States

        private enum TextBoxState
        {
            Normal, Focused, PointerOver, KeyboardShown
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_contentElement = GetTemplateChild("ContentElement") as ItemsControl;
            var showKeyboardButton = GetTemplateChild("ShowKeyboardButton") as Button;
            if (showKeyboardButton == null || showKeyboardButton.Flyout == null)
                return;

            showKeyboardButton.Flyout.Opening += (sender, o) =>
            {
                VisualStateManager.GoToState(this, "Focused", false);
                m_textBoxState = TextBoxState.KeyboardShown;
            };
            showKeyboardButton.Flyout.Closed += (sender, o) => Focus(FocusState.Programmatic);


            var gridView = ((Flyout) showKeyboardButton.Flyout).Content as GridView;
            if (gridView == null)
                return;

            // Add event handler for Keyboard letter click
            gridView.ItemClick += (sender, args) =>
            {
                var viewModel = args.ClickedItem as ButtonViewModel;
                if (viewModel == null)
                    return;

                InsertLetter(viewModel.Label);
            };
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (m_textBoxState == TextBoxState.Focused)
            {
                VisualStateManager.GoToState(this, "Normal", false);
                m_textBoxState = TextBoxState.Normal;
            }
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            e.Handled = true;

            if (m_textBoxState == TextBoxState.Normal)
            {
                VisualStateManager.GoToState(this, "PointerOver", false);
                m_textBoxState = TextBoxState.PointerOver;
            }
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            e.Handled = true;

            if (m_textBoxState == TextBoxState.PointerOver)
            {
                VisualStateManager.GoToState(this, "Normal", false);
                m_textBoxState = TextBoxState.Normal;
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            m_textBoxState = TextBoxState.Focused;
        }
    }

    public class ButtonViewModel
    {
        public char Label { get; set; }
    }
}
