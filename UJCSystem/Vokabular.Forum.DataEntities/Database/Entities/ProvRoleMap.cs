using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class ProvRoleMap : ClassMapping<ProvRole> {
        
        public ProvRoleMap() {
			Table("yaf_prov_Role");
			Schema("dbo");
			Lazy(false);
			Id(x => x.RoleID, map => map.Generator(Generators.Assigned));
			Property(x => x.ApplicationID, map => map.NotNullable(true));
			Property(x => x.RoleName, map => map.NotNullable(true));
			Property(x => x.RoleNameLwd, map => map.NotNullable(true));
        }
    }
}
