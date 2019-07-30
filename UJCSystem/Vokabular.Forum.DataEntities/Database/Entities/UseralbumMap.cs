using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class UseralbumMap : ClassMapping<UserAlbum> {
        
        public UseralbumMap() {
			Table("yaf_UserAlbum");
			Schema("dbo");
			Lazy(false);
			Id(x => x.AlbumID, map => map.Generator(Generators.Identity));
			Property(x => x.UserID, map => map.NotNullable(true));
			Property(x => x.Title);
			Property(x => x.CoverImageID);
			Property(x => x.Updated, map => map.NotNullable(true));
			Bag(x => x.UserAlbumImages, colmap =>  { colmap.Key(x => x.Column("AlbumID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
