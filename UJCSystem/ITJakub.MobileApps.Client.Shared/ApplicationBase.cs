using Windows.UI.Xaml;

namespace ITJakub.MobileApps.Client.Shared
{
    public abstract class ApplicationBase
    {
        public abstract string Name { get; }

        public abstract ApplicationBaseViewModel ApplicationViewModel { get; }

        public abstract DataTemplate ApplicationDataTemplate { get; }

        public abstract bool IsChatSupported { get; }
    }
}
