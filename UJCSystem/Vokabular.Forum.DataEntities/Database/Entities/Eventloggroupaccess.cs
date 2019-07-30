using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class EventLogGroupAccess {
        public int GroupID { get; set; }
        public int EventTypeID { get; set; }
        public Group Group { get; set; }
        public string EventTypeName { get; set; }
        public bool DeleteAccess { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as EventLogGroupAccess;
			if (t == null) return false;
			if (GroupID == t.GroupID
			 && EventTypeID == t.EventTypeID)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ GroupID.GetHashCode();
			hash = (hash * 397) ^ EventTypeID.GetHashCode();

			return hash;
        }
        #endregion
    }
}
