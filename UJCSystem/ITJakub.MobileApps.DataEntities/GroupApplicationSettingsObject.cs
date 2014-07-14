using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ITJakub.MobileApps.DataEntities
{
    //Each group has own settings
    public class GroupApplicationSettingsObject
    {
        public virtual long Id { get; set; }

        public virtual Application ApplicationId { get; set; }

        public virtual UsersGroup UsersGroupId { get; set; }

        public virtual string Key { get; set; }
     
        public virtual string Value { get; set; }
    }
}
