using System;
using System.Collections.Generic;

namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Topic {

        public Topic(Forum forum, DateTime posted, string topicText, short priority, User user)
        {
            Forum = forum;
            Posted = posted;
            TopicText = topicText;
            Priority = priority;
            User = user;
        }

        public Topic()
        {}

        public int TopicID { get; set; }
        public Forum Forum { get; set; }
        public User User { get; set; }
        public PollGroupCluster PollGroupCluster { get; set; }
        public Topic MovedTopic { get; set; }
        public Message LastMessage { get; set; }
        public User LastUser { get; set; }
        public string UserName { get; set; }
        public string UserDisplayName { get; set; }
        public DateTime Posted { get; set; }
        public string TopicText { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Styles { get; set; }
        public DateTime? LinkDate { get; set; }
        public int Views { get; set; } = 0;
        public short Priority { get; set; }
        public DateTime? LastPosted { get; set; }
        public string LastUserName { get; set; }
        public string LastUserDisplayName { get; set; }
        public int NumPosts { get; set; } = 0;
        public int Flags { get; set; } = 0;
        public bool? IsDeleted { get; set; }
        public bool? IsQuestion { get; set; }
        public int? AnswerMessageId { get; set; }
        public int? LastMessageFlags { get; set; }
        public string TopicImage { get; set; }
        public IList<Active> Actives { get; set; }
        public IList<FavoriteTopic> FavoriteTopics { get; set; }
        public IList<Forum> Forums { get; set; }
        public IList<Message> Messages { get; set; }
        public IList<NntpTopic> NntpTopics { get; set; }
        public IList<Topic> Topics { get; set; }
        public IList<TopicReadTracking> TopicReadTrackings { get; set; }
        public IList<WatchTopic> WatchTopics { get; set; }
    }
}
