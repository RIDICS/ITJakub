using System.Collections.Generic;

namespace ITJakub.MobileApps.DataEntities.Database.Entities
{
    //Should save data for multiplayer with more groups
    public class GroupMultiplayer
    {
        public virtual long Id { get; set; }

        //TODO Just Idea
        public virtual IList<UsersGroup> ApplicationGroups { get; set; }
    }
}
