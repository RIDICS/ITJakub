using System;
using System.Text;
using System.Collections.Generic;


namespace ITJakub.MobileApps.DataEntities.Database.Entities {
    
    public class Databaseversion {
        public virtual int Id { get; set; }
        public virtual string Databaseversionval { get; set; }
        public virtual string Solutionversion { get; set; }
        public virtual DateTime Upgradedate { get; set; }
        public virtual string Upgradeuser { get; set; }
    }
}
