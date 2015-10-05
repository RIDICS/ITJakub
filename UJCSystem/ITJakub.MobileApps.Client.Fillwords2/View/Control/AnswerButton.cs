using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel.Data;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel.Enum;

namespace ITJakub.MobileApps.Client.Fillwords2.View.Control
{
    [TemplatePart(Name = "StackPanel", Type = typeof(StackPanel))]
    public sealed class AnswerButton : Windows.UI.Xaml.Controls.Control
    {
        private StackPanel m_stackPanel;

        public AnswerButton()
        {
            DefaultStyleKey = typeof(AnswerButton);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(AnswerButton), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register("Options", typeof(SimpleWordOptionsViewModel), typeof(AnswerButton), new PropertyMetadata(null, OptionsPropertyChanged));
        
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public SimpleWordOptionsViewModel Options
        {
            get { return (SimpleWordOptionsViewModel) GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        private static void OptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as AnswerButton;
            if (button == null || button.m_stackPanel == null || button.Options == null)
                return;

            var stackPanel = button.m_stackPanel;
            var wordOption = button.Options;
            var word = wordOption.CorrectAnswer;

            var startPosition = 0;
            var endPosition = 0;
            stackPanel.Children.Clear();
            
            foreach (var letterOptionViewModel in wordOption.Options)
            {
                endPosition = letterOptionViewModel.StartPosition;
                CreateTextControl(stackPanel, word, startPosition, endPosition);

                switch (letterOptionViewModel.AnswerTypeViewModel.AnswerType)
                {
                    case AnswerType.Selection:
                        //TODO
                        break;
                    default:
                        CreateFillControl(stackPanel, letterOptionViewModel);
                        break;
                }
                startPosition = letterOptionViewModel.EndPosition;
            }

            endPosition = word.Length;
            CreateTextControl(stackPanel, word, startPosition, endPosition);
        }

        private static void CreateTextControl(StackPanel stackPanel, string word, int startPosition, int endPosition)
        {
            if (startPosition < endPosition)
            {
                var text = word.Substring(startPosition, endPosition - startPosition);
                var textBlock = new TextBlock
                {
                    Text = text
                };
                stackPanel.Children.Add(textBlock);
            }
        }

        private static void CreateFillControl(StackPanel stackPanel, LetterOptionViewModel letterOptionViewModel)
        {
            var textBox = new TextBox
            {
                MinWidth = 20
            };
            stackPanel.Children.Add(textBox);
        }


        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_stackPanel = GetTemplateChild("StackPanel") as StackPanel;

            OptionsPropertyChanged(this, null);
        }
    }
}
