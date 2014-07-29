using System;
using System.Reflection;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Shared
{
    public abstract class ApplicationBase
    {
        public ISynchronizeCommunication ApplicationCommunication { get; set; }

        public abstract string Name { get; }

        public abstract ApplicationBaseViewModel ApplicationViewModel { get; }

        public abstract Type ApplicationDataTemplate { get; }

        public abstract bool IsChatSupported { get; }

        public abstract BitmapImage Icon { get; }

        private string AssemblyName
        {
            get { return GetType().GetTypeInfo().Assembly.GetName().Name; }
        }

        protected Uri BaseUri
        {
            get { return new Uri(string.Format("ms-appx:///{0}/", AssemblyName)); }
        }
    }
}