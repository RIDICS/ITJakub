using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class ForumAccess {
        public int GroupID { get; set; }
        public int ForumID { get; set; }
        public Group Group { get; set; }
        public Forum Forum { get; set; }
        public AccessMask AccessMask { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as ForumAccess;
			if (t == null) return false;
			if (GroupID == t.GroupID
			 && ForumID == t.ForumID)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ GroupID.GetHashCode();
			hash = (hash * 397) ^ ForumID.GetHashCode();

			return hash;
        }
        #endregion
    }
}
