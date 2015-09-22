namespace ITJakub.MobileApps.DataEntities.ExternalEntities
{
    public interface ISynchronizedObjectEntity
    {
        string ExternalId { get; }

        long GroupId { get; }

        string Data { get; set; }
    }
}