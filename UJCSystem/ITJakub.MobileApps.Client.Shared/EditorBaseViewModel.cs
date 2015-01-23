using System;
using GalaSoft.MvvmLight;

namespace ITJakub.MobileApps.Client.Shared
{
    public abstract class EditorBaseViewModel : ViewModelBase
    {
        public Action GoBack { get; set; }
    }
}
