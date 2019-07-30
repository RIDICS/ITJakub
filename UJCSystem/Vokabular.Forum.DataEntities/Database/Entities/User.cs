using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class User {
        public User() {
			Actives = new List<Active>();
			AdminPageUserAccesses = new List<AdminPageUserAccess>();
			Attachments = new List<Attachment>();
			Buddies = new List<Buddy>();
			CheckEmails = new List<CheckEmail>();
			EventLogs = new List<EventLog>();
			FavoriteTopics = new List<FavoriteTopic>();
			Forums = new List<Forum>();
			ForumReadTrackings = new List<ForumReadTracking>();
			Messages = new List<Message>();
			PMessages = new List<PMessage>();
			ReputationVotesFromUser = new List<ReputationVote>();
			ReputationVotesToUser = new List<ReputationVote>();
			Thanks = new List<Thank>();
			UserTopics = new List<Topic>();
			TopicReadTrackings = new List<TopicReadTracking>();
			UserForums = new List<UserForum>();
			Groups = new List<Group>();
			UserMedals = new List<UserMedal>();
			UserPMessages = new List<UserPMessage>();
			UserProfiles = new List<UserProfile>();
			WatchForums = new List<WatchForum>();
			WatchTopics = new List<WatchTopic>();
        }
        public int UserID { get; set; }
        public Board Board { get; set; }
        public Rank Rank { get; set; }
        public string ProviderUserKey { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime Joined { get; set; }
        public DateTime LastVisit { get; set; }
        public string IP { get; set; }
        public int NumPosts { get; set; }
        public string TimeZone { get; set; }
        public string Avatar { get; set; }
        public string Signature { get; set; }
        public byte[] AvatarImage { get; set; }
        public string AvatarImageType { get; set; }
        public DateTime? Suspended { get; set; }
        public string SuspendedReason { get; set; }
        public int SuspendedBy { get; set; }
        public string LanguageFile { get; set; }
        public string ThemeFile { get; set; }
        public string TextEditor { get; set; }
        public bool OverridedefaultThemes { get; set; }
        public bool PMNotification { get; set; }
        public bool AutoWatchTopics { get; set; }
        public bool DailyDigest { get; set; }
        public int? NotificationType { get; set; }
        public int Flags { get; set; }
        public int Points { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsGuest { get; set; }
        public bool? IsCaptchaExcluded { get; set; }
        public bool? IsActiveExcluded { get; set; }
        public bool? IsDST { get; set; }
        public bool? IsDirty { get; set; }
        public string Culture { get; set; }
        public bool IsFacebookUser { get; set; }
        public bool IsTwitterUser { get; set; }
        public string UserStyle { get; set; }
        public int StyleFlags { get; set; }
        public bool? IsUserStyle { get; set; }
        public bool? IsGroupStyle { get; set; }
        public bool? IsRankStyle { get; set; }
        public bool IsGoogleUser { get; set; }
        public IList<Active> Actives { get; set; }
        public IList<AdminPageUserAccess> AdminPageUserAccesses { get; set; }
        public IList<Attachment> Attachments { get; set; }
        public IList<Buddy> Buddies { get; set; }
        public IList<CheckEmail> CheckEmails { get; set; }
        public IList<EventLog> EventLogs { get; set; }
        public IList<FavoriteTopic> FavoriteTopics { get; set; }
        public IList<Forum> Forums { get; set; }
        public IList<ForumReadTracking> ForumReadTrackings { get; set; }
        public IList<Message> Messages { get; set; }
        public IList<PMessage> PMessages { get; set; }
        public IList<ReputationVote> ReputationVotesToUser { get; set; }
        public IList<ReputationVote> ReputationVotesFromUser { get; set; }
        public IList<Thank> Thanks { get; set; }
        public IList<Topic> UserTopics { get; set; }
        public IList<TopicReadTracking> TopicReadTrackings { get; set; }
        public IList<UserForum> UserForums { get; set; }
        public IList<Group> Groups { get; set; }
        public IList<UserMedal> UserMedals { get; set; }
        public IList<UserPMessage> UserPMessages { get; set; }
        public IList<UserProfile> UserProfiles { get; set; }
        public IList<WatchForum> WatchForums { get; set; }
        public IList<WatchTopic> WatchTopics { get; set; }
    }
}
