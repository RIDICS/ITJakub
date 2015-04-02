namespace ITJakub.MobileApps.DataEntities.Database.Entities
{
    public enum GroupState : short
    {
        Created = 0,
        AcceptMembers = 1,
        WaitingForStart = 2,
        Running = 3,
        Paused = 4,
        Closed = 5
    }
}
