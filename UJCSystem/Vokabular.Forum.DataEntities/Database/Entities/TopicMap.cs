using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class TopicMap : ClassMapping<Topic> {
        
        public TopicMap() {
			Table("yaf_Topic");
			Schema("dbo");
			Lazy(false);
			Id(x => x.TopicID, map => map.Generator(Generators.Identity));
			Property(x => x.UserName);
			Property(x => x.UserDisplayName);
			Property(x => x.Posted, map => map.NotNullable(true));
			Property(x => x.TopicText, map =>
			{
			    map.NotNullable(true);
                map.Column("Topic");
			});
			Property(x => x.Description);
			Property(x => x.Status);
			Property(x => x.Styles);
			Property(x => x.LinkDate);
			Property(x => x.Views, map => map.NotNullable(true));
			Property(x => x.Priority, map => map.NotNullable(true));
			Property(x => x.LastPosted);
			Property(x => x.LastUserName);
			Property(x => x.LastUserDisplayName);
			Property(x => x.NumPosts, map => map.NotNullable(true));
			Property(x => x.Flags, map => map.NotNullable(true));
			Property(x => x.IsDeleted, m =>
			{
			    m.Insert(false);
			    m.Update(false);
			});
			Property(x => x.IsQuestion, m =>
			{
			    m.Insert(false);
			    m.Update(false);
			});
			Property(x => x.AnswerMessageId);
			Property(x => x.LastMessageFlags);
			Property(x => x.TopicImage);
			ManyToOne(x => x.Forum, map => 
			{
				map.Column("ForumID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.User, map => 
			{
				map.Column("UserID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.PollGroupCluster, map => 
			{
				map.Column("PollID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.MovedTopic, map => 
			{
				map.Column("TopicMovedID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.LastMessage, map => 
			{
				map.Column("LastMessageID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.LastUser, map => 
			{
				map.Column("LastUserID");
				map.Cascade(Cascade.None);
			});

			Bag(x => x.Actives, colmap =>  { colmap.Key(x => x.Column("TopicID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.FavoriteTopics, colmap =>  { colmap.Key(x => x.Column("TopicID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Forums, colmap =>  { colmap.Key(x => x.Column("LastTopicID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Messages, colmap =>  { colmap.Key(x => x.Column("TopicID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.NntpTopics, colmap =>  { colmap.Key(x => x.Column("TopicID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Topics, colmap =>  { colmap.Key(x => x.Column("TopicMovedID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.TopicReadTrackings, colmap =>  { colmap.Key(x => x.Column("TopicID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.WatchTopics, colmap =>  { colmap.Key(x => x.Column("TopicID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
