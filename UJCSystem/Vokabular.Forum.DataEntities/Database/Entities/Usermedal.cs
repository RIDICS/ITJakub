using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class UserMedal {
        public int UserID { get; set; }
        public int MedalID { get; set; }
        public User User { get; set; }
        public Medal Medal { get; set; }
        public string Message { get; set; }
        public bool Hide { get; set; }
        public bool OnlyRibbon { get; set; }
        public byte SortOrder { get; set; }
        public DateTime DateAwarded { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as UserMedal;
			if (t == null) return false;
			if (UserID == t.UserID
			 && MedalID == t.MedalID)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ UserID.GetHashCode();
			hash = (hash * 397) ^ MedalID.GetHashCode();

			return hash;
        }
        #endregion
    }
}
