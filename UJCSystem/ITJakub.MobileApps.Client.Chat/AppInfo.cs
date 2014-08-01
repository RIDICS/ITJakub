using System;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Chat.DataService;
using ITJakub.MobileApps.Client.Chat.View;
using ITJakub.MobileApps.Client.Chat.ViewModel;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Communication;

namespace ITJakub.MobileApps.Client.Chat
{
    [MobileApplication(ApplicationType.Chat)]
    public class AppInfo : ApplicationBase
    {
        public AppInfo(ISynchronizeCommunication applicationCommunication) : base(applicationCommunication)
        {
            DataService = new ChatDataService(applicationCommunication);
        }

        public override string Name
        {
            get { return "Chat"; }
        }

        public override ApplicationBaseViewModel ApplicationViewModel
        {
            get { return new ChatViewModel(DataService); }
        }

        private IChatDataService DataService { get; set; }

        public override Type ApplicationDataTemplate
        {
            get { return typeof (ChatView); }
        }

        public override ApplicationRoleType ApplicationRoleType
        {
            get { return ApplicationRoleType.SupportApp;}
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