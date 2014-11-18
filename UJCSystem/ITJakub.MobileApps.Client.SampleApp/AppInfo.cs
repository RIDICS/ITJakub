using System;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.SampleApp
{
    [MobileApplication(ApplicationType.SampleApp)]
    public class AppInfo : ApplicationBase
    {
        private readonly SampleDataService m_dataService;

        public AppInfo(ISynchronizeCommunication applicationCommunication) : base(applicationCommunication)
        {
            m_dataService = new SampleDataService(ApplicationCommunication);
        }

        public override string Name
        {
            get { return "Sample application name"; }
        }

        public override ApplicationBaseViewModel ApplicationViewModel
        {
            get { return new SampleViewModel(m_dataService); }
        }

        public override Type ApplicationDataTemplate
        {
            get { return typeof (SampleView); }
        }

        public override ViewModelBase EditorViewModel
        {
            get { return new SampleEditorViewModel(m_dataService); }
        }

        public override Type EditorDataTemplate
        {
            get { return typeof (SampleEditorView); }
        }

        public override ApplicationRoleType ApplicationRoleType
        {
            get { return ApplicationRoleType.MainApp; }
        }

        public override ApplicationCategory ApplicationCategory
        {
            get { return ApplicationCategory.Education; }
        }

        public override bool IsChatSupported
        {
            get { return true; }
        }

        public override BitmapImage Icon
        {
            get
            {
                var uri = new Uri(BaseUri, "Icon/file-128.png");
                var image = new BitmapImage(uri);
                return image;
            }
        }

    
    }
}