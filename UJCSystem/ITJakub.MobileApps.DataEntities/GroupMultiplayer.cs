using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.MobileApps.DataEntities
{
    public class GroupMultiplayer
    {
        public virtual long Id { get; set; }

        //TODO Just Idea
        public virtual IList<UsersGroup> ApplicationGroups { get; set; }
    }
}
