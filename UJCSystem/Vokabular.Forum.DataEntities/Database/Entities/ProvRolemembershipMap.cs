using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class ProvRolemembershipMap : ClassMapping<ProvRoleMembership> {
        
        public ProvRolemembershipMap() {
			Table("yaf_prov_RoleMembership");
			Schema("dbo");
			Lazy(false);
			ComposedId(compId =>
				{
					compId.Property(x => x.RoleID, m => m.Column("RoleID"));
					compId.Property(x => x.UserID, m => m.Column("UserID"));
				});
        }
    }
}
