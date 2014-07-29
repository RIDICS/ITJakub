using System;
using System.Collections.Generic;

namespace ITJakub.MobileApps.Client.Shared.Communication
{
    public interface ISynchronizeCommunication
    {

        void SendObject(ApplicationType applicationType, string objectType, string objectValue);


        List<string> GetSynchronizedObjects(ApplicationType applicationType, DateTime from,string objectType = null);

        //todo more required methods

    }
}