using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace ITJakub.MobileApps.Client.Shared.Control
{
    [ContentProperty(Name = "Content")]
    public sealed class Expander : Windows.UI.Xaml.Controls.Control
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof (string), typeof (Expander), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof (object), typeof (Expander), new PropertyMetadata(null));

        public Expander()
        {
            DefaultStyleKey = typeof(Expander);
        }

        public string Header
        {
            get { return (string) GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
    }
}
