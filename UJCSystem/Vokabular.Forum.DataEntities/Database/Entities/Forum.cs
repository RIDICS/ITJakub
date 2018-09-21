using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Forum {

        public Forum(string name, Category category, short sortOrder, int numTopics = 0, int numPosts = 0, int flags = 0)
        {
            Name = name;
            Category = category;
            SortOrder = sortOrder;
            NumTopics = numTopics;
            NumPosts = numPosts;
            Flags = flags;
        }

        public Forum() {
			Actives = new List<Active>();
			Forums = new List<Forum>();
			ForumAccesses = new List<ForumAccess>();
			ForumReadTrackings = new List<ForumReadTracking>();
			NntpForums = new List<NntpForum>();
			Topics = new List<Topic>();
			UserForums = new List<UserForum>();
			WatchForums = new List<WatchForum>();
        }
        public int ForumID { get; set; }
        public Category Category { get; set; }
        public Forum ParentForum { get; set; }
        public Topic LastTopic { get; set; }
        public Message LastMessage { get; set; }
        public User LastUser { get; set; }
        public PollGroupCluster PollGroupCluster { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short SortOrder { get; set; }
        public DateTime? LastPosted { get; set; }
        public string LastUserName { get; set; }
        public string LastUserDisplayName { get; set; }
        public int NumTopics { get; set; }
        public int NumPosts { get; set; }
        public string RemoteURL { get; set; }
        public int Flags { get; set; }
        public bool? IsLocked { get; set; }
        public bool? IsHidden { get; set; }
        public bool? IsNoCount { get; set; }
        public bool? IsModerated { get; set; }
        public string ThemeURL { get; set; }
        public string ImageURL { get; set; }
        public string Styles { get; set; }
        public User User { get; set; }
        public int? ModeratedPostCount { get; set; }
        public bool IsModeratedNewTopicOnly { get; set; }
        public int? ExternalId { get; set; }
        public IList<Active> Actives { get; set; }
        public IList<Forum> Forums { get; set; }
        public IList<ForumAccess> ForumAccesses { get; set; }
        public IList<ForumReadTracking> ForumReadTrackings { get; set; }
        public IList<NntpForum> NntpForums { get; set; }
        public IList<Topic> Topics { get; set; }
        public IList<UserForum> UserForums { get; set; }
        public IList<WatchForum> WatchForums { get; set; }
    }
}
