using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class ProvApplication {
        public System.Guid ApplicationID { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationNameLwd { get; set; }
        public string Description { get; set; }
    }
}
