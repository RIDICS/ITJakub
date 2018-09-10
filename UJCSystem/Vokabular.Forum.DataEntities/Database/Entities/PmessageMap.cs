using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class PmessageMap : ClassMapping<PMessage> {
        
        public PmessageMap() {
			Table("yaf_PMessage");
			Schema("dbo");
			Lazy(false);
			Id(x => x.PMessageID, map => map.Generator(Generators.Identity));
			Property(x => x.ReplyTo);
			Property(x => x.Created, map => map.NotNullable(true));
			Property(x => x.Subject, map => map.NotNullable(true));
			Property(x => x.Body, map => map.NotNullable(true));
			Property(x => x.Flags, map => map.NotNullable(true));
			ManyToOne(x => x.User, map => 
			{
				map.Column("FromUserID");
				map.PropertyRef("UserID");
				map.Cascade(Cascade.None);
			});

			Bag(x => x.UserPMessages, colmap =>  { colmap.Key(x => x.Column("PMessageID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
