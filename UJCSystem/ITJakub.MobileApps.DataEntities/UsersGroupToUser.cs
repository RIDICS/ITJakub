using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.MobileApps.DataEntities
{
    public class UserBelongsToUsersGroup
    {
        public virtual long Id { get; set; }

        public virtual User UserId { get; set; }

        public virtual UsersGroup UsersGroupId { get; set; }
    }
}