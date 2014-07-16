namespace ITJakub.MobileApps.DataEntities.Database.Entities
{
    //Each group has own settings
    public class GroupApplicationSettingsObject
    {
        public virtual long Id { get; set; }

        public virtual Application ApplicationId { get; set; }

        public virtual UsersGroup UsersGroupId { get; set; }

        public virtual string Key { get; set; }
     
        public virtual string Value { get; set; }
    }
}
