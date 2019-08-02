using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class MessageMap : ClassMapping<Message> {
        
        public MessageMap() {
			Table("yaf_Message");
			Schema("dbo");
			Lazy(false);
			Id(x => x.MessageID, map => map.Generator(Generators.Identity));
			Property(x => x.Position, map => map.NotNullable(true));
			Property(x => x.Indent, map => map.NotNullable(true));
			Property(x => x.UserName);
			Property(x => x.UserDisplayName);
			Property(x => x.Posted, map => map.NotNullable(true));
			Property(x => x.MessageText, map =>
			{
			    map.NotNullable(true);
                map.Column("Message");
			});
			Property(x => x.IP, map => map.NotNullable(true));
			Property(x => x.Edited);
			Property(x => x.Flags, map => map.NotNullable(true));
			Property(x => x.EditReason);
			Property(x => x.IsModeratorChanged, m =>
			{
			    m.Insert(false);
			    m.Update(false);
			});
			Property(x => x.DeleteReason);
			Property(x => x.ExternalMessageId);
			Property(x => x.ReferenceMessageId);
			Property(x => x.IsDeleted, m =>
			{
			    m.Insert(false);
			    m.Update(false);
			});
			Property(x => x.IsApproved, m =>
			{
			    m.Insert(false);
			    m.Update(false);
			});
			Property(x => x.BlogPostID);
			Property(x => x.EditedBy);
			ManyToOne(x => x.Topic, map => 
			{
				map.Column("TopicID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.ReplyTo, map => 
			{
				map.Column("ReplyTo");
				map.PropertyRef("MessageID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.User, map => 
			{
				map.Column("UserID");
				map.Cascade(Cascade.None);
			});

			Bag(x => x.Forums, colmap =>  { colmap.Key(x => x.Column("LastMessageID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Messages, colmap =>  { colmap.Key(x => x.Column("ReplyTo")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.MessageHistories, colmap =>  { colmap.Key(x => x.Column("MessageID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Topics, colmap =>  { colmap.Key(x => x.Column("LastMessageID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
