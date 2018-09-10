using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class ForumreadtrackingMap : ClassMapping<ForumReadTracking> {
        
        public ForumreadtrackingMap() {
			Table("yaf_ForumReadTracking");
			Schema("dbo");
			Lazy(false);
			ComposedId(compId =>
				{
					compId.Property(x => x.UserID, m => m.Column("UserID"));
					compId.Property(x => x.ForumID, m => m.Column("ForumID"));
				});
			Property(x => x.LastAccessDate, map => map.NotNullable(true));
			ManyToOne(x => x.User, map => 
			{
				map.Column("UserID");
				map.PropertyRef("UserID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.Forum, map => 
			{
				map.Column("ForumID");
				map.PropertyRef("ForumID");
				map.Cascade(Cascade.None);
			});

        }
    }
}
