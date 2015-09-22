using System.Collections.Generic;

namespace ITJakub.MobileApps.DataEntities.ExternalEntities
{
    public interface ISynchronizedObjectDao
    {
        void Save(ISynchronizedObjectEntity entity);        
    
        void Delete(string externalObjectId, long groupId);        

        ISynchronizedObjectEntity FindByObjectExternalIdAndGroup(string objectExternalId, long groupId);

        ISynchronizedObjectEntity GetNewEntity(long groupId, string data);

        void DeleteSynchronizedObjects(long groupId, IEnumerable<string> externalIds);
    }
}