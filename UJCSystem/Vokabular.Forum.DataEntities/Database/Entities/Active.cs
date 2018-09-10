using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Active {
        public string SessionID { get; set; }
        public int BoardID { get; set; }
        public Board Board { get; set; }
        public User User { get; set; }
        public Forum Forum { get; set; }
        public Topic Topic { get; set; }
        public string IP { get; set; }
        public DateTime Login { get; set; }
        public DateTime LastActive { get; set; }
        public string Location { get; set; }
        public string Browser { get; set; }
        public string Platform { get; set; }
        public int? Flags { get; set; }
        public string ForumPage { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as Active;
			if (t == null) return false;
			if (SessionID == t.SessionID
			 && BoardID == t.BoardID)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ SessionID.GetHashCode();
			hash = (hash * 397) ^ BoardID.GetHashCode();

			return hash;
        }
        #endregion
    }
}
