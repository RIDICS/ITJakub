using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.MobileApps.DataEntities
{
    public class GroupStateObject
    {
        public virtual long Id { get; set; }

        public virtual UsersGroup UsersGroupId { get; set; }

        public virtual DateTime CreateTime { get; set; }
        
        //data are stored as string
        public virtual string Data { get; set; }

    }
}
