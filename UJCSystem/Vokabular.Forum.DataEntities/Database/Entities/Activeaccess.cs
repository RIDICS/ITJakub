using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class ActiveAccess {
        public int UserID { get; set; }
        public int ForumID { get; set; }
        public int BoardID { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsForumModerator { get; set; }
        public bool IsModerator { get; set; }
        public bool ReadAccess { get; set; }
        public bool PostAccess { get; set; }
        public bool ReplyAccess { get; set; }
        public bool PriorityAccess { get; set; }
        public bool PollAccess { get; set; }
        public bool VoteAccess { get; set; }
        public bool ModeratorAccess { get; set; }
        public bool EditAccess { get; set; }
        public bool DeleteAccess { get; set; }
        public bool UploadAccess { get; set; }
        public bool DownloadAccess { get; set; }
        public DateTime? LastActive { get; set; }
        public bool IsGuestX { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as ActiveAccess;
			if (t == null) return false;
			if (UserID == t.UserID
			 && ForumID == t.ForumID)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ UserID.GetHashCode();
			hash = (hash * 397) ^ ForumID.GetHashCode();

			return hash;
        }
        #endregion
    }
}
