using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class AdminPageUserAccess {
        public int UserID { get; set; }
        public string PageName { get; set; }
        public User User { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as AdminPageUserAccess;
			if (t == null) return false;
			if (UserID == t.UserID
			 && PageName == t.PageName)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ UserID.GetHashCode();
			hash = (hash * 397) ^ PageName.GetHashCode();

			return hash;
        }
        #endregion
    }
}
