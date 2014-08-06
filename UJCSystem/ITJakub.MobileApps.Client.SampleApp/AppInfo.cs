using System;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.SampleApp
{
    [MobileApplication(ApplicationType.SampleApp)]
    public class AppInfo : ApplicationBase
    {
        public AppInfo(ISynchronizeCommunication applicationCommunication) : base(applicationCommunication)
        {
        }

        public override string Name
        {
            get { return "Sample application name"; }
        }

        public override ApplicationBaseViewModel ApplicationViewModel
        {
            get { return new SampleViewModel(new SampleDataService(ApplicationCommunication)); }
        }

        public override Type ApplicationDataTemplate
        {
            get { return typeof (SampleView); }
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