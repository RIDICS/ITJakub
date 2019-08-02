using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Vokabular.ForumSite.DataEntities.Database.Entities;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    
    public class UserMap : ClassMapping<User> {
        
        public UserMap() {
			Table("yaf_User");
			Schema("dbo");
			Lazy(false);
			Id(x => x.UserID, map => map.Generator(Generators.Identity));
			Property(x => x.ProviderUserKey);
			Property(x => x.Name, map => { map.NotNullable(true); map.Unique(true); });
			Property(x => x.DisplayName, map => map.NotNullable(true));
			Property(x => x.Password, map => map.NotNullable(true));
			Property(x => x.Email);
			Property(x => x.Joined, map => map.NotNullable(true));
			Property(x => x.LastVisit, map => map.NotNullable(true));
			Property(x => x.IP);
			Property(x => x.NumPosts, map => map.NotNullable(true));
			Property(x => x.TimeZone);
			Property(x => x.Avatar);
			Property(x => x.Signature);
			Property(x => x.AvatarImage);
			Property(x => x.AvatarImageType);
			Property(x => x.Suspended);
			Property(x => x.SuspendedReason);
			Property(x => x.SuspendedBy, map => map.NotNullable(true));
			Property(x => x.LanguageFile);
			Property(x => x.ThemeFile);
			Property(x => x.TextEditor);
			Property(x => x.OverridedefaultThemes, map => map.NotNullable(true));
			Property(x => x.PMNotification, map => map.NotNullable(true));
			Property(x => x.AutoWatchTopics, map => map.NotNullable(true));
			Property(x => x.DailyDigest, map => map.NotNullable(true));
			Property(x => x.NotificationType);
			Property(x => x.Flags, map => map.NotNullable(true));
			Property(x => x.Points, map => map.NotNullable(true));
			Property(x => x.IsApproved);
			Property(x => x.IsGuest);
			Property(x => x.IsCaptchaExcluded);
			Property(x => x.IsActiveExcluded);
			Property(x => x.IsDST);
			Property(x => x.IsDirty);
			Property(x => x.Culture);
			Property(x => x.IsFacebookUser, map => map.NotNullable(true));
			Property(x => x.IsTwitterUser, map => map.NotNullable(true));
			Property(x => x.UserStyle);
			Property(x => x.StyleFlags, map => map.NotNullable(true));
			Property(x => x.IsUserStyle);
			Property(x => x.IsGroupStyle);
			Property(x => x.IsRankStyle);
			Property(x => x.IsGoogleUser, map => map.NotNullable(true));
			ManyToOne(x => x.Board, map => { map.Column("BoardID"); map.Cascade(Cascade.None); });

			ManyToOne(x => x.Rank, map => 
			{
				map.Column("RankID");
				map.Cascade(Cascade.None);
			});

           

			Bag(x => x.Actives, colmap =>  { colmap.Key(x => x.Column("UserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.AdminPageUserAccesses, colmap =>  { colmap.Key(x => x.Column("UserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Attachments, colmap =>  { colmap.Key(x => x.Column("UserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Buddies, colmap =>  { colmap.Key(x => x.Column("FromUserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.CheckEmails, colmap =>  { colmap.Key(x => x.Column("UserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.EventLogs, colmap =>  { colmap.Key(x => x.Column("UserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.FavoriteTopics, colmap =>  { colmap.Key(x => x.Column("UserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Forums, colmap =>  { colmap.Key(x => x.Column("LastUserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.ForumReadTrackings, colmap =>  { colmap.Key(x => x.Column("UserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Messages, colmap =>  { colmap.Key(x => x.Column("UserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.PMessages, colmap =>  { colmap.Key(x => x.Column("FromUserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.ReputationVotesFromUser, colmap =>  { colmap.Key(x => x.Column("ReputationFromUserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.ReputationVotesToUser, colmap =>  { colmap.Key(x => x.Column("ReputationToUserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.Thanks, colmap =>  { colmap.Key(x => x.Column("ThanksFromUserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.UserTopics, colmap =>  { colmap.Key(x => x.Column("UserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.TopicReadTrackings, colmap =>  { colmap.Key(x => x.Column("UserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.UserForums, colmap =>  { colmap.Key(x => x.Column("UserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.UserMedals, colmap =>  { colmap.Key(x => x.Column("UserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.UserPMessages, colmap =>  { colmap.Key(x => x.Column("UserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.UserProfiles, colmap =>  { colmap.Key(x => x.Column("UserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.WatchForums, colmap =>  { colmap.Key(x => x.Column("UserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
			Bag(x => x.WatchTopics, colmap =>  { colmap.Key(x => x.Column("UserID")); colmap.Inverse(true); }, map => { map.OneToMany(); });
            Bag(x => x.Groups, colmap =>
            {
                colmap.Table("yaf_UserGroup");
                colmap.Cascade(Cascade.None);
                colmap.Key(x => x.Column("UserID"));
            }, map => { map.ManyToMany(p => p.Column("GroupID")); });
        }
    }
}
