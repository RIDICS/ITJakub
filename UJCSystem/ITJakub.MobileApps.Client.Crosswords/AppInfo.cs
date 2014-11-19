using System;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Crosswords.DataService;
using ITJakub.MobileApps.Client.Crosswords.View;
using ITJakub.MobileApps.Client.Crosswords.ViewModel;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Crosswords
{
    [MobileApplication(ApplicationType.Crosswords)]
    public class AppInfo : ApplicationBase
    {
        private readonly CrosswordsDataService m_dataService;

        public AppInfo(ISynchronizeCommunication applicationCommunication) : base(applicationCommunication)
        {
            m_dataService = new CrosswordsDataService(applicationCommunication);
        }

        public override string Name
        {
            get { return "Křížovky"; }
        }

        public override ApplicationBaseViewModel ApplicationViewModel
        {
            get { return new CrosswordsViewModel(m_dataService); }
        }

        public override Type ApplicationDataTemplate
        {
            get { return typeof (CrosswordsView); }
        }

        public override EditorBaseViewModel EditorViewModel
        {
            get { return null; } // TODO editor
        }

        public override Type EditorDataTemplate
        {
            get { return null; }
        }

        public override ApplicationRoleType ApplicationRoleType
        {
            get { return ApplicationRoleType.MainApp; }
        }

        public override ApplicationCategory ApplicationCategory
        {
            get { return ApplicationCategory.Game; }
        }

        public override bool IsChatSupported
        {
            get { return true; }
        }

        public override BitmapImage Icon
        {
            get { return new BitmapImage(new Uri(BaseUri, "Icon/gantt_chart-128.png")); }
        }
    }
}
