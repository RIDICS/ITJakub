namespace ITJakub.MobileApps.DataEntities.ExternalEntities
{
    public interface ITaskDao
    {
        ITaskEntity GetNewEntity(long taskId, int appId, string data);

        void Save(ITaskEntity taskEntity);

        ITaskEntity FindByIdAndAppId(long taskId, int appId);
    }
}