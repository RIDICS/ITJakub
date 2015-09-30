using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Shared.ViewModel
{
    public abstract class TaskPreviewBaseViewModel : ViewModelBase
    {
        public abstract void ShowTask(string data);
    }
}