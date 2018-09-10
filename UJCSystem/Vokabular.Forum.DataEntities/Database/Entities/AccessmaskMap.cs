using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class AccessmaskMap : ClassMapping<AccessMask> {
        
        public AccessmaskMap() {
			Table("yaf_AccessMask");
			Schema("dbo");
			Lazy(false);
			Id(x => x.AccessMaskID, map => map.Generator(Generators.Identity));
			Property(x => x.Name, map => map.NotNullable(true));
			Property(x => x.Flags, map => map.NotNullable(true));
			Property(x => x.SortOrder, map => map.NotNullable(true));
			ManyToOne(x => x.Board, map => { map.Column("BoardID"); map.Cascade(Cascade.None); });

			Bag(x => x.ForumAccesses, colmap =>  { colmap.Key(x => x.Column("AccessMaskID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.UserForums, colmap =>  { colmap.Key(x => x.Column("AccessMaskID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
