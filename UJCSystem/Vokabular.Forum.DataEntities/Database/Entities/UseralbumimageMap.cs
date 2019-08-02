using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class UseralbumimageMap : ClassMapping<UserAlbumImage> {
        
        public UseralbumimageMap() {
			Table("yaf_UserAlbumImage");
			Schema("dbo");
			Lazy(false);
			Id(x => x.ImageID, map => map.Generator(Generators.Identity));
			Property(x => x.Caption);
			Property(x => x.FileName, map => map.NotNullable(true));
			Property(x => x.Bytes, map => map.NotNullable(true));
			Property(x => x.ContentType);
			Property(x => x.Uploaded, map => map.NotNullable(true));
			Property(x => x.Downloads, map => map.NotNullable(true));
			ManyToOne(x => x.UserAlbum, map => { map.Column("AlbumID"); map.Cascade(Cascade.None); });

        }
    }
}
