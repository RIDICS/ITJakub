using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class ProvRoleMembership {
        public System.Guid RoleID { get; set; }
        public string UserID { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as ProvRoleMembership;
			if (t == null) return false;
			if (RoleID == t.RoleID
			 && UserID == t.UserID)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ RoleID.GetHashCode();
			hash = (hash * 397) ^ UserID.GetHashCode();

			return hash;
        }
        #endregion
    }
}
