using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class UserGroup {
        public int UserID { get; set; }
        public int GroupID { get; set; }
        public User User { get; set; }
        public Group Group { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as UserGroup;
			if (t == null) return false;
			if (UserID == t.UserID
			 && GroupID == t.GroupID)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ UserID.GetHashCode();
			hash = (hash * 397) ^ GroupID.GetHashCode();

			return hash;
        }
        #endregion
    }
}
