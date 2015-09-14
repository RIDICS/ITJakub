using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.Shared.ViewModel
{
    public abstract class AdminBaseViewModel : ViewModelBase
    {
        public abstract void SetTask(string data);
        public abstract void InitializeCommunication();
        public abstract void UpdateGroupMembers(IEnumerable<UserInfo> members);
    }
}