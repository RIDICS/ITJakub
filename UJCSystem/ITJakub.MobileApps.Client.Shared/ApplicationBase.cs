using System;
using System.Reflection;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Shared
{
    public abstract class ApplicationBase
    {
        protected ApplicationBase(ISynchronizeCommunication applicationCommunication)
        {
            ApplicationCommunication = applicationCommunication;
        }


        public ISynchronizeCommunication ApplicationCommunication { get; private set; }

        public abstract string Name { get; }

        public abstract ApplicationBaseViewModel ApplicationViewModel { get; }

        public abstract Type ApplicationDataTemplate { get; }

        public abstract EditorBaseViewModel EditorViewModel { get; }

        public abstract Type EditorDataTemplate { get; }

        public abstract ApplicationRoleType ApplicationRoleType { get; }

        public abstract ApplicationCategory ApplicationCategory { get; }

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