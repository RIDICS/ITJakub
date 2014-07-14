using System.Collections.Generic;

namespace ITJakub.MobileApps.DataEntities
{

    //Its like Session
    public class UsersGroup
    {
        public virtual long Id { get; set; }

        public virtual string CodeToEnter { get; set; }

        public virtual Application ApplicationId { get; set; }


        //gets from bag
        public virtual IEnumerable<User> Users { get; set; }

    }
}
