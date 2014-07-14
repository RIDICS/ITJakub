using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.MobileApps.DataEntities
{
    public class ApplicationSettingsObject
    {

        public virtual long Id { get; set; }

        public virtual long ApplicationId { get; set; }

        //object is stored as string
        public virtual string SettingsObject { get; set; }
    }
}
