using System;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Fillwords.DataService;
using ITJakub.MobileApps.Client.Fillwords.View;
using ITJakub.MobileApps.Client.Fillwords.ViewModel;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Fillwords
{
    [MobileApplication(ApplicationType.Fillwords)]
    public class AppInfo : ApplicationBase
    {
        private readonly FillwordsDataService m_dataService;

        public AppInfo(ISynchronizeCommunication applicationCommunication) : base(applicationCommunication)
        {
            m_dataService = new FillwordsDataService(applicationCommunication);
        }

        public override string Name
        {
            get { return "Doplňovačky"; }
        }

        public override ApplicationBaseViewModel ApplicationViewModel
        {
            get { return new FillwordsViewModel(m_dataService); }
        }

        public override Type ApplicationDataTemplate
        {
            get { return typeof (FillwordsView); }
        }

        public override EditorBaseViewModel EditorViewModel
        {
            get { return new EditorViewModel(m_dataService); }
        }

        public override Type EditorDataTemplate
        {
            get { return typeof (EditorView); }
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
            get { return new BitmapImage(new Uri(BaseUri, "Icon/pen-128.png")); }
        }
    }
}
