// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITJakub.MobileApps.Client.Hangman.View
{
    public sealed partial class WordControl
    {
        public WordControl()
        {
            InitializeComponent();
        }

        public string Word
        {
            get { return (string) GetValue(WordProperty); }
            set { SetValue(WordProperty, value); }
        }

        public static readonly DependencyProperty WordProperty = DependencyProperty.Register("Word", typeof(string), typeof(WordControl), new PropertyMetadata(string.Empty, WordPropertyChanged));

        private static void WordPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var wordControl = source as WordControl;
            var newWord = e.NewValue.ToString();
            if (wordControl == null)
                return;

            CreateSubviews(wordControl, newWord);
        }

        private static void CreateSubviews(WordControl wordControl, string sentence)
        {
            wordControl.WrapPanel.Children.Clear();
            var words = sentence.Split(' ');

            foreach (var word in words)
            {
                var stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(20,0,20,10)
                };

                foreach (var letter in word)
                {
                    var letterControl = new LetterControl { Letter = letter };
                    stackPanel.Children.Add(letterControl);
                }

                wordControl.WrapPanel.Children.Add(stackPanel);
            }
        }
    }
}
