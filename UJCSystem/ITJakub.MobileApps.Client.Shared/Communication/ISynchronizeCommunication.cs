using System;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.DataContracts;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Shared.Communication
{
    public interface ISynchronizeCommunication
    {
        void SendObject(ApplicationType applicationType, string objectType, string objectValue);

        ObservableCollection<ObjectDetails> GetSynchronizedObjects(ApplicationType applicationType, DateTime from, string objectType = null);

        //todo more required methods

    }
}