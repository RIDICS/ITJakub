using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Hangman.View.Control
{
    [TemplatePart(Type = typeof(TextBox), Name = "TextBox")]
    [TemplatePart(Type = typeof(GridView), Name = "GridView")]
    [TemplatePart(Type = typeof(Button), Name = "ShowKeyboardButton")]
    public sealed class SpecialTextBox : Windows.UI.Xaml.Controls.Control
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof (string), typeof (SpecialTextBox), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty KeyboardLettersProperty = DependencyProperty.Register("KeyboardLetters", typeof (string), typeof (SpecialTextBox), new PropertyMetadata(string.Empty, OnKeyboardLettersChanged));
        private NoLostFocusTextBox m_textBox;
        private GridView m_gridView;
        private Button m_showKeyboardButton;
        
        public SpecialTextBox()
        {
            DefaultStyleKey = typeof(SpecialTextBox);
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string KeyboardLetters
        {
            get { return (string) GetValue(KeyboardLettersProperty); }
            set { SetValue(KeyboardLettersProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_textBox = GetTemplateChild("TextBox") as NoLostFocusTextBox;
            m_gridView = GetTemplateChild("GridView") as GridView;
            m_showKeyboardButton = GetTemplateChild("ShowKeyboardButton") as Button;

            if (m_gridView != null)
                m_gridView.ItemClick += OnKeyboardButtonClick;

            OnKeyboardLettersChanged(this, null);

            RegisterEventsForHandlingFocus();
        }
        
        private void OnKeyboardButtonClick(object sender, ItemClickEventArgs e)
        {
            var letter = (char) e.ClickedItem;
            var cursorPosition = m_textBox.SelectionStart;
            m_textBox.Text = m_textBox.Text.Insert(cursorPosition, new string(letter, 1));
            m_textBox.SelectionStart = cursorPosition + 1;
            m_textBox.Focus(FocusState.Programmatic);
        }

        private static void OnKeyboardLettersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var specialTextBox = d as SpecialTextBox;
            if (specialTextBox == null || specialTextBox.m_gridView == null)
                return;

            specialTextBox.m_gridView.ItemsSource = new ObservableCollection<char>(specialTextBox.KeyboardLetters);
        }
        
        private void RegisterEventsForHandlingFocus()
        {
            if (m_showKeyboardButton.Flyout == null)
                return;

            m_showKeyboardButton.Flyout.Opened += (sender, o) =>
            {
                m_textBox.Focus(FocusState.Programmatic);
            };
            m_showKeyboardButton.Flyout.Closed += (sender, o) =>
            {
                m_textBox.Focus(FocusState.Programmatic);
            };
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            m_textBox.LostFocusProgrammatically();
        }
    }
}
