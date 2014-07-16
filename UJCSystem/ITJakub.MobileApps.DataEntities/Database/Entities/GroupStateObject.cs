using System;

namespace ITJakub.MobileApps.DataEntities.Database.Entities
{
    public class GroupStateObject
    {
        public virtual long Id { get; set; }

        public virtual UsersGroup UsersGroupId { get; set; }

        public virtual DateTime CreateTime { get; set; }
        
        public virtual string Key { get; set; }

        public virtual string Value { get; set; }

    }
}
