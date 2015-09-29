using Windows.UI.Xaml;

namespace ITJakub.MobileApps.Client.Hangman.View.Control
{
    public sealed partial class WordControl
    {
        public static readonly DependencyProperty LetterWidthProperty = DependencyProperty.Register("LetterWidth", typeof (double), typeof (WordControl), new PropertyMetadata(40));
        public static readonly DependencyProperty LetterHeightProperty = DependencyProperty.Register("LetterHeight", typeof (double), typeof (WordControl), new PropertyMetadata(47));
        public static readonly DependencyProperty LineHeightProperty = DependencyProperty.Register("LineHeight", typeof (double), typeof (WordControl), new PropertyMetadata(7, OnPropertyChanged));
        public static readonly DependencyProperty LetterBorderThicknessProperty = DependencyProperty.Register("LetterBorderThickness", typeof (Thickness), typeof (WordControl), new PropertyMetadata(new Thickness(0,0,0,7)));
        
        public WordControl()
        {
            InitializeComponent();
            FontSize = 30;
        }

        public double LetterWidth
        {
            get { return (double) GetValue(LetterWidthProperty); }
            set { SetValue(LetterWidthProperty, value); }
        }

        public double LetterHeight
        {
            get { return (double) GetValue(LetterHeightProperty); }
            set { SetValue(LetterHeightProperty, value); }
        }

        public double LineHeight
        {
            get { return (double) GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        public Thickness LetterBorderThickness
        {
            get { return (Thickness)GetValue(LetterBorderThicknessProperty); }
            set { SetValue(LetterBorderThicknessProperty, value); }
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var wordControl = d as WordControl;
            if (wordControl == null)
                return;

            wordControl.LetterBorderThickness = new Thickness(0, 0, 0, wordControl.LineHeight);
        }
    }
}
