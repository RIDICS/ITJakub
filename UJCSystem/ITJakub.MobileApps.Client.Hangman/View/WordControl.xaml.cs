// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

using Windows.UI.Xaml;

namespace ITJakub.MobileApps.Client.Hangman.View
{
    public sealed partial class WordControl
    {
        public WordControl()
        {
            InitializeComponent();
        }
        //TODO wrap words

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

            wordControl.WrapGrid.Children.Clear();
            foreach (var letter in newWord)
            {
                var letterControl = new LetterControl {Letter = letter};
                wordControl.WrapGrid.Children.Add(letterControl);
            }
        }
    }
}
