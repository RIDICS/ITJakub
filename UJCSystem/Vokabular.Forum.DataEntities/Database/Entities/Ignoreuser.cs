using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class IgnoreUser {
        public int UserID { get; set; }
        public int IgnoredUserID { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as IgnoreUser;
			if (t == null) return false;
			if (UserID == t.UserID
			 && IgnoredUserID == t.IgnoredUserID)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ UserID.GetHashCode();
			hash = (hash * 397) ^ IgnoredUserID.GetHashCode();

			return hash;
        }
        #endregion
    }
}
