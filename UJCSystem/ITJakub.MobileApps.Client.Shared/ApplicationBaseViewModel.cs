using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Shared
{
    public abstract class ApplicationBaseViewModel : ViewModelBase
    {
        public abstract void InitializeCommunication();
        public abstract void SetTask(string data);
        public abstract void StopCommunication();
    }
}
