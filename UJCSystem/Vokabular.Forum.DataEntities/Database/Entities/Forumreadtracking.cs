using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class ForumReadTracking {
        public int UserID { get; set; }
        public int ForumID { get; set; }
        public User User { get; set; }
        public Forum Forum { get; set; }
        public DateTime LastAccessDate { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as ForumReadTracking;
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
