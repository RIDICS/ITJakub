using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class MedalMap : ClassMapping<Medal> {
        
        public MedalMap() {
			Table("yaf_Medal");
			Schema("dbo");
			Lazy(false);
			Id(x => x.MedalID, map => map.Generator(Generators.Identity));
			Property(x => x.BoardID, map => map.NotNullable(true));
			Property(x => x.Name, map => map.NotNullable(true));
			Property(x => x.Description, map => map.NotNullable(true));
			Property(x => x.Message, map => map.NotNullable(true));
			Property(x => x.Category);
			Property(x => x.MedalURL, map => map.NotNullable(true));
			Property(x => x.RibbonURL);
			Property(x => x.SmallMedalURL, map => map.NotNullable(true));
			Property(x => x.SmallRibbonURL);
			Property(x => x.SmallMedalWidth, map => map.NotNullable(true));
			Property(x => x.SmallMedalHeight, map => map.NotNullable(true));
			Property(x => x.SmallRibbonWidth);
			Property(x => x.SmallRibbonHeight);
			Property(x => x.SortOrder, map => map.NotNullable(true));
			Property(x => x.Flags, map => map.NotNullable(true));
			Bag(x => x.GroupMedals, colmap =>  { colmap.Key(x => x.Column("MedalID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.UserMedals, colmap =>  { colmap.Key(x => x.Column("MedalID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
