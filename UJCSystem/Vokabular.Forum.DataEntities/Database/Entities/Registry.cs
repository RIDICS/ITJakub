using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Registry {
        public int RegistryID { get; set; }
        public Board Board { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
