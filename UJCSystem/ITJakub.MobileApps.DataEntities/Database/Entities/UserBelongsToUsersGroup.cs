namespace ITJakub.MobileApps.DataEntities.Database.Entities
{
    /// <summary>
    /// Relationship table between user and group
    /// </summary>
    public class UserBelongsToUsersGroup
    {
        public virtual long Id { get; set; }

        public virtual User UserId { get; set; }

        public virtual UsersGroup UsersGroupId { get; set; }
    }
}