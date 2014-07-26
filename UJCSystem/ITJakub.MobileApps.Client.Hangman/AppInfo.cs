using System;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Hangman
{
    [MobileApplication(ApplicationType.Hangman)]
    public class AppInfo : ApplicationBase
    {
        public override string Name
        {
            get { return "Šibenice"; }
        }

        public override ApplicationBaseViewModel ApplicationViewModel
        {
            get { return new HangmanViewModel(); }
        }

        public override Type ApplicationDataTemplate
        {
            get { return typeof (HangmanView); }
        }

        public override bool IsChatSupported
        {
            get { return false; }
        }

        public override BitmapImage Icon
        {
            get
            {
                var uri = new Uri(BaseUri, "Icon/scythe-128.png");
                return new BitmapImage(uri);
            }
        }
    }
}
