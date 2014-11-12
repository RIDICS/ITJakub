using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235
using Windows.UI.Xaml.Input;

namespace ITJakub.MobileApps.Client.Crosswords.View
{
    [TemplatePart(Name = "ShowKeyboardButton", Type = typeof(Button))]
    public class CrosswordTextBoxControl : TextBox
    {
        private TextBoxState m_textBoxState;

        public CrosswordTextBoxControl()
        {
            DefaultStyleKey = typeof(CrosswordTextBoxControl);
            SelectionChanged += OnSelectionChanged;
            TextChanged += OnTextChanged;
        }

        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            // Move cursor to correct position
            CursorMargin = new Thickness((Height - 2)*SelectionStart - 1, 0, 0, 0);

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

        public static readonly DependencyProperty CursorMarginProperty = DependencyProperty.Register("CursorMargin",
            typeof(Thickness), typeof(CrosswordTextBoxControl), new PropertyMetadata(new Thickness(-1,0,0,0)));

        public static readonly DependencyProperty KeyboardLettersProperty = DependencyProperty.Register("KeyboardLetters",
            typeof (string), typeof (CrosswordTextBoxControl), new PropertyMetadata(default(string)));


        // Section for handling Visual States

        private enum TextBoxState
        {
            Normal, Focused, PointerOver, KeyboardShown
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var showKeyboardButton = GetTemplateChild("ShowKeyboardButton") as Button;
            if (showKeyboardButton == null || showKeyboardButton.Flyout == null)
                return;

            showKeyboardButton.Flyout.Opening += (sender, o) =>
            {
                VisualStateManager.GoToState(this, "Focused", false);
                m_textBoxState = TextBoxState.KeyboardShown;
            };
            showKeyboardButton.Flyout.Closed += (sender, o) => Focus(FocusState.Programmatic);
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
}
