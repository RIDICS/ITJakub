using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class ReputationVote {
        public int ReputationFromUserID { get; set; }
        public int ReputationToUserID { get; set; }
        public User ReputationFromUser { get; set; }
        public User ReputationToUser { get; set; }
        public DateTime VoteDate { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as ReputationVote;
			if (t == null) return false;
			if (ReputationFromUserID == t.ReputationFromUserID
			 && ReputationToUserID == t.ReputationToUserID)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ ReputationFromUserID.GetHashCode();
			hash = (hash * 397) ^ ReputationToUserID.GetHashCode();

			return hash;
        }
        #endregion
    }
}
