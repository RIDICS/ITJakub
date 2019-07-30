using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class PollgroupclusterMap : ClassMapping<PollGroupCluster> {
        
        public PollgroupclusterMap() {
			Table("yaf_PollGroupCluster");
			Schema("dbo");
			Lazy(false);
			Id(x => x.PollGroupID, map => map.Generator(Generators.Identity));
			Property(x => x.UserID, map => map.NotNullable(true));
			Property(x => x.Flags, map => map.NotNullable(true));
			Property(x => x.IsBound);
			Bag(x => x.Categories, colmap =>  { colmap.Key(x => x.Column("PollGroupID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Forums, colmap =>  { colmap.Key(x => x.Column("PollGroupID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Polls, colmap =>  { colmap.Key(x => x.Column("PollGroupID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Topics, colmap =>  { colmap.Key(x => x.Column("PollID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
