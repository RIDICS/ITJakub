using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class ProvRole {
        public System.Guid RoleID { get; set; }
        public System.Guid ApplicationID { get; set; }
        public string RoleName { get; set; }
        public string RoleNameLwd { get; set; }
    }
}
