// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

using Windows.UI.Xaml;

namespace ITJakub.MobileApps.Client.Hangman.View
{
    public sealed partial class LetterControl
    {
        public LetterControl()
        {
            InitializeComponent();
        }

        public char Letter
        {
            get { return (char) GetValue(LetterProperty); }
            set { SetValue(LetterProperty, value); }
        }

        public static readonly DependencyProperty LetterProperty = DependencyProperty.Register("Letter", typeof (char),
            typeof (LetterControl), new PropertyMetadata('.', LetterPropertyChanged));
        
        private static void LetterPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var letterControl = source as LetterControl;
            var newLetter = (char) e.NewValue;
            if (letterControl != null)
            {
                letterControl.LetterTextBlock.Text = newLetter.ToString();
                letterControl.Border.BorderThickness = newLetter == ' ' ? new Thickness(0) : new Thickness(0, 0, 0, 10);
            }
        }
    }
}
