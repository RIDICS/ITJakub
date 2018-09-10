using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class AttachmentMap : ClassMapping<Attachment> {
        
        public AttachmentMap() {
			Table("yaf_Attachment");
			Schema("dbo");
			Lazy(false);
			Id(x => x.AttachmentID, map => map.Generator(Generators.Identity));
			Property(x => x.MessageID, map => map.NotNullable(true));
			Property(x => x.FileName, map => map.NotNullable(true));
			Property(x => x.Bytes, map => map.NotNullable(true));
			Property(x => x.ContentType);
			Property(x => x.Downloads, map => map.NotNullable(true));
			Property(x => x.FileData);
			ManyToOne(x => x.User, map => 
			{
				map.Column("UserID");
				map.Cascade(Cascade.None);
			});

        }
    }
}
