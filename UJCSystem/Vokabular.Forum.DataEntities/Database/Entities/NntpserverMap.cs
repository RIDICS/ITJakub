using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class NntpserverMap : ClassMapping<NntpServer> {
        
        public NntpserverMap() {
			Table("yaf_NntpServer");
			Schema("dbo");
			Lazy(false);
			Id(x => x.NntpServerID, map => map.Generator(Generators.Identity));
			Property(x => x.Name, map => map.NotNullable(true));
			Property(x => x.Address, map => map.NotNullable(true));
			Property(x => x.Port);
			Property(x => x.UserName);
			Property(x => x.UserPass);
			ManyToOne(x => x.Board, map => { map.Column("BoardID"); map.Cascade(Cascade.None); });

			Bag(x => x.NntpForums, colmap =>  { colmap.Key(x => x.Column("NntpServerID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
