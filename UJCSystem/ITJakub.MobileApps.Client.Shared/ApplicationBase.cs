using System;
using Windows.UI.Xaml.Media.Imaging;

namespace ITJakub.MobileApps.Client.Shared
{
    public abstract class ApplicationBase
    {
        public abstract string Name { get; }

        public abstract ApplicationBaseViewModel ApplicationViewModel { get; }

        public abstract Type ApplicationDataTemplate { get; }

        public abstract bool IsChatSupported { get; }

        public abstract BitmapImage Icon { get; }

        public string Assembly { get; set; }

        protected Uri BaseUri
        {
            get
            {
                return new Uri(string.Format("ms-appx:///{0}/", Assembly));
            }
        }
    }
}
