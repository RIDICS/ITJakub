using Windows.UI.Xaml;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.SampleApp
{
    [MobileApplication(ApplicationType.Crosswords)]
    public class AppInfo : ApplicationBase
    {
        public override string Name
        {
            get { return "Sample app"; }
        }

        public override ApplicationBaseViewModel ApplicationViewModel
        {
            get { return new ApplicationBaseViewModel(); }
        }

        public override DataTemplate ApplicationDataTemplate
        {
            get
            {
                var a =  new DataTemplate();
                return a;
            }
        }

        public override bool IsChatSupported
        {
            get { return true; }
        }
    }
}