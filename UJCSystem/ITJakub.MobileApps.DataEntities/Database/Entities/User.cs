using System.Collections.Generic;

namespace ITJakub.MobileApps.DataEntities.Database.Entities
{

    //Base user info
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
