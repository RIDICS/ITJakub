using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class BoardMap : ClassMapping<Board> {
        
        public BoardMap() {
			Table("yaf_Board");
			Schema("dbo");
			Lazy(false);
			Id(x => x.BoardID, map => map.Generator(Generators.Identity));
			Property(x => x.Name, map => map.NotNullable(true));
			Property(x => x.AllowThreaded, map => map.NotNullable(true));
			Property(x => x.MembershipAppName);
			Property(x => x.RolesAppName);
			Bag(x => x.AccessMasks, colmap =>  { colmap.Key(x => x.Column("BoardID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Actives, colmap =>  { colmap.Key(x => x.Column("BoardID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.BannedEmails, colmap =>  { colmap.Key(x => x.Column("BoardID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.BannedIPs, colmap =>  { colmap.Key(x => x.Column("BoardID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.BannedNames, colmap =>  { colmap.Key(x => x.Column("BoardID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.BBCodes, colmap =>  { colmap.Key(x => x.Column("BoardID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Categories, colmap =>  { colmap.Key(x => x.Column("BoardID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Extensions, colmap =>  { colmap.Key(x => x.Column("BoardId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Groups, colmap =>  { colmap.Key(x => x.Column("BoardID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.NntpServers, colmap =>  { colmap.Key(x => x.Column("BoardID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Ranks, colmap =>  { colmap.Key(x => x.Column("BoardID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Registries, colmap =>  { colmap.Key(x => x.Column("BoardID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Smileys, colmap =>  { colmap.Key(x => x.Column("BoardID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Users, colmap =>  { colmap.Key(x => x.Column("BoardID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
