using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Shared.Communication
{
    public interface ISynchronizeCommunication
    {
        Task SendObjectAsync(ApplicationType applicationType, string objectType, string objectValue);

        Task<IEnumerable<ObjectDetails>> GetObjectsAsync(ApplicationType applicationType, DateTime since, string objectType = null);

        //todo more required methods
    }
}