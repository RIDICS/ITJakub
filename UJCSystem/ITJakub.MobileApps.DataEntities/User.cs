using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.MobileApps.DataEntities
{
    public class User
    {
        public virtual long Id { get; set; }

        public virtual string Email { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual Role RoleId { get; set; }

        public virtual Institution InstitutionId { get; set; }



        //gets from bag
        public virtual IList<UsersGroup> UserGroups { get; set; } 

    }
}
