using System;
using System.Collections.Generic;

namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Message {

        public Message(Topic topic, User user, DateTime posted, string messageText)
        {
            Topic = topic;
            User = user;
            UserDisplayName = user.DisplayName;
            Posted = posted;
            MessageText = messageText;
            Flags = 534;
            IP = "";
            Position = 0;
            Indent = 0;
        }

        public Message() {
			Forums = new List<Forum>();
			Messages = new List<Message>();
			MessageHistories = new List<MessageHistory>();
			Topics = new List<Topic>();
        }

        public int MessageID { get; set; }
        public Topic Topic { get; set; }
        public Message ReplyTo { get; set; }
        public User User { get; set; }
        public int Position { get; set; }
        public int Indent { get; set; }
        public string UserName { get; set; }
        public string UserDisplayName { get; set; }
        public DateTime Posted { get; set; }
        public string MessageText { get; set; }
        public string IP { get; set; }
        public DateTime? Edited { get; set; }
        public int Flags { get; set; }
        public string EditReason { get; set; }
        public bool IsModeratorChanged { get; set; }
        public string DeleteReason { get; set; }
        public string ExternalMessageId { get; set; }
        public string ReferenceMessageId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsApproved { get; set; }
        public string BlogPostID { get; set; }
        public int? EditedBy { get; set; }
        public IList<Forum> Forums { get; set; }
        public IList<Message> Messages { get; set; }
        public IList<MessageHistory> MessageHistories { get; set; }
        public IList<Topic> Topics { get; set; }
    }
}
