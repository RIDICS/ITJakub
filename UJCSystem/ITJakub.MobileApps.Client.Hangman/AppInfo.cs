using System;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Hangman.DataService;
using ITJakub.MobileApps.Client.Hangman.View;
using ITJakub.MobileApps.Client.Hangman.ViewModel;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Hangman
{
    [MobileApplication(ApplicationType.Hangman)]
    public class AppInfo : ApplicationBase
    {
        private readonly HangmanDataService m_dataService;

        public AppInfo(ISynchronizeCommunication applicationCommunication) : base(applicationCommunication)
        {
            m_dataService = new HangmanDataService(applicationCommunication);
        }

        public override string Name
        {
            get { return "Šibenice"; }
        }

        public override ApplicationBaseViewModel ApplicationViewModel
        {
            get { return new HangmanViewModel(m_dataService); }
        }

        public override Type ApplicationDataTemplate
        {
            get { return typeof (HangmanView); }
        }

        public override ViewModelBase EditorViewModel
        {
            get { return null; } // TODO editor
        }

        public override Type EditorDataTemplate
        {
            get { return null; }
        }

        public override ApplicationRoleType ApplicationRoleType
        {
            get { return ApplicationRoleType.MainApp;}
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
            get
            {
                var uri = new Uri(BaseUri, "Icon/scythe-128.png");
                return new BitmapImage(uri);
            }
        }
    }
}
