using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
