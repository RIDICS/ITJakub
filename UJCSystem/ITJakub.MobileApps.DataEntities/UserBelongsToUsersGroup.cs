using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.MobileApps.DataEntities
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