using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class ProvApplicationMap : ClassMapping<ProvApplication> {
        
        public ProvApplicationMap() {
			Table("yaf_prov_Application");
			Schema("dbo");
			Lazy(false);
			Id(x => x.ApplicationID, map => map.Generator(Generators.Assigned));
			Property(x => x.ApplicationName);
			Property(x => x.ApplicationNameLwd);
			Property(x => x.Description);
        }
    }
}
