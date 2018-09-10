using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class RankMap : ClassMapping<Rank> {
        
        public RankMap() {
			Table("yaf_Rank");
			Schema("dbo");
			Lazy(false);
			Id(x => x.RankID, map => map.Generator(Generators.Identity));
			Property(x => x.Name, map => { map.NotNullable(true); map.Unique(true); });
			Property(x => x.MinPosts);
			Property(x => x.RankImage);
			Property(x => x.Flags, map => map.NotNullable(true));
			Property(x => x.PMLimit);
			Property(x => x.Style);
			Property(x => x.SortOrder, map => map.NotNullable(true));
			Property(x => x.Description);
			Property(x => x.UsrSigChars, map => map.NotNullable(true));
			Property(x => x.UsrSigBBCodes);
			Property(x => x.UsrSigHTMLTags);
			Property(x => x.UsrAlbums, map => map.NotNullable(true));
			Property(x => x.UsrAlbumImages, map => map.NotNullable(true));
			ManyToOne(x => x.Board, map => { map.Column("BoardID"); map.Cascade(Cascade.None); });

			Bag(x => x.Users, colmap =>  { colmap.Key(x => x.Column("RankID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
