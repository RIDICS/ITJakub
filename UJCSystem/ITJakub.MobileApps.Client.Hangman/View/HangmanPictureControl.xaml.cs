// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;

namespace ITJakub.MobileApps.Client.Hangman.View
{
    public sealed partial class HangmanPictureControl
    {
        public HangmanPictureControl()
        {
            InitializeComponent();
            HangmanShapes = new Shape[]
            {
                RightLeg,
                LeftLeg,
                RightArm,
                LeftArm,
                Body,
                Head,
                Rope,
                Construction3,
                Construction2,
                Construction1,
                Base
            };
            FaceShapes = new Shape[]
            {
                Mouth,
                RightEye,
                LeftEye
            };
            foreach (var shape in HangmanShapes)
            {
                shape.Visibility = Visibility.Collapsed;
            }
            foreach (var shape in FaceShapes)
            {
                shape.Visibility = Visibility.Collapsed;
            }
        }

        public Shape[] HangmanShapes { get; set; }

        public Shape[] FaceShapes { get; set; }
        
        public int Lives
        {
            get { return (int) GetValue(LivesProperty); }
            set { SetValue(LivesProperty, value); }
        }

        public static readonly DependencyProperty LivesProperty = DependencyProperty.Register("Lives", typeof (int),
            typeof (HangmanPictureControl), new PropertyMetadata(11, LivesPropertyChanged));

        private static void LivesPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var pictureControl = (HangmanPictureControl) source;
            var lives = (int) e.NewValue;
            
            var shapes = pictureControl.HangmanShapes;
            for (int i = 0; i < shapes.Length; i++)
            {
                shapes[i].Visibility = i >= lives ? Visibility.Visible : Visibility.Collapsed;
            }

            var faceVisibility = lives == 0 ? Visibility.Visible : Visibility.Collapsed;
            foreach (var faceShape in pictureControl.FaceShapes)
            {
                faceShape.Visibility = faceVisibility;
            }
        }
    }
}
