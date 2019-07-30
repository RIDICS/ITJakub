using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class ForumAccess {
        public Group Group { get; set; }
        public Forum Forum { get; set; }
        public AccessMask AccessMask { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as ForumAccess;
			if (t == null) return false;
			if (Group.GroupID == t.Group.GroupID
			 && Forum.ForumID == t.Forum.ForumID)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ Group.GroupID.GetHashCode();
			hash = (hash * 397) ^ Forum.ForumID.GetHashCode();

			return hash;
        }
        #endregion
    }
}
