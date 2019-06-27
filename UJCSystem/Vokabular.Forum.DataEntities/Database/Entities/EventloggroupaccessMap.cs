using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class EventloggroupaccessMap : ClassMapping<EventLogGroupAccess> {
        
        public EventloggroupaccessMap() {
			Table("yaf_EventLogGroupAccess");
			Schema("dbo");
			Lazy(false);
			ComposedId(compId =>
				{
					compId.Property(x => x.GroupID, m => m.Column("GroupID"));
					compId.Property(x => x.EventTypeID, m => m.Column("EventTypeID"));
				});
			Property(x => x.EventTypeName, map => map.NotNullable(true));
			Property(x => x.DeleteAccess, map => map.NotNullable(true));
			ManyToOne(x => x.Group, map => 
			{
				map.Column("GroupID");
				map.Cascade(Cascade.None);
                map.Insert(false);
                map.Update(false);
            });

        }
    }
}
