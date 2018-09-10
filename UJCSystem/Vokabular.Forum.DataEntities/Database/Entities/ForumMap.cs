using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class ForumMap : ClassMapping<Forum> {
        
        public ForumMap() {
			Table("yaf_Forum");
			Schema("dbo");
			Lazy(false);
			Id(x => x.ForumID, map => map.Generator(Generators.Identity));
			Property(x => x.Name, map => map.NotNullable(true));
			Property(x => x.Description);
			Property(x => x.SortOrder, map => map.NotNullable(true));
			Property(x => x.LastPosted);
			Property(x => x.LastUserName);
			Property(x => x.LastUserDisplayName);
			Property(x => x.NumTopics, map => map.NotNullable(true));
			Property(x => x.NumPosts, map => map.NotNullable(true));
			Property(x => x.RemoteURL);
			Property(x => x.Flags, map => map.NotNullable(true));
            Property(x => x.IsLocked, m =>
            {
                m.Insert(false);
                m.Update(false);
            });
			Property(x => x.IsHidden, m =>
			{
			    m.Insert(false);
			    m.Update(false);
			});
			Property(x => x.IsNoCount, m =>
			{
			    m.Insert(false);
			    m.Update(false);
			});
			Property(x => x.IsModerated, m =>
			{
			    m.Insert(false);
			    m.Update(false);
			});

			Property(x => x.ThemeURL);
			Property(x => x.ImageURL);
			Property(x => x.Styles);
			Property(x => x.ModeratedPostCount);
			Property(x => x.IsModeratedNewTopicOnly, map => map.NotNullable(true));
			ManyToOne(x => x.Category, map => 
			{
				map.Column("CategoryID");
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.ParentForum, map => 
			{
				map.Column("ParentID");
				map.NotNullable(false);
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.LastTopic, map => 
			{
				map.Column("LastTopicID");
				map.NotNullable(false);
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.LastMessage, map => 
			{
				map.Column("LastMessageID");
				map.NotNullable(false);
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.LastUser, map => 
			{
				map.Column("LastUserID");
				map.NotNullable(false);
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.PollGroupCluster, map => 
			{
				map.Column("PollGroupID");
				map.NotNullable(false);
				map.Cascade(Cascade.None);
			});

            ManyToOne(x => x.User, map =>
            {
                map.Column("UserID");
                map.Cascade(Cascade.None);
            });

            Bag(x => x.Actives, colmap =>  { colmap.Key(x => x.Column("ForumID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Forums, colmap =>  { colmap.Key(x => x.Column("ParentID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.ForumAccesses, colmap =>  { colmap.Key(x => x.Column("ForumID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.ForumReadTrackings, colmap =>  { colmap.Key(x => x.Column("ForumID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.NntpForums, colmap =>  { colmap.Key(x => x.Column("ForumID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Topics, colmap =>  { colmap.Key(x => x.Column("ForumID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.UserForums, colmap =>  { colmap.Key(x => x.Column("ForumID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.WatchForums, colmap =>  { colmap.Key(x => x.Column("ForumID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}
