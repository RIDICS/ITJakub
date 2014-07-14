using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.MobileApps.DataEntities
{
    public class Institution
    {
        public virtual long Id { get; set; }

        public virtual string Name { get; set; }
    }
}
