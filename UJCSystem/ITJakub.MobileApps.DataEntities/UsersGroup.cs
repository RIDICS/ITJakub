using System.Collections.Generic;

namespace ITJakub.MobileApps.DataEntities
{

    //group is more users solving the same task
    public class UsersGroup
    {
        public virtual long Id { get; set; }

        public virtual string CodeToEnter { get; set; }

        public virtual Application ApplicationId { get; set; }


        //gets from bag
        public virtual IList<User> Users { get; set; }

        public virtual IList<GroupStateObject> GroupStateObjects { get; set; }

    }
}
