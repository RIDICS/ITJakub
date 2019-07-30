using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class GroupMedal {
        public int GroupID { get; set; }
        public int MedalID { get; set; }
        public Group Group { get; set; }
        public Medal Medal { get; set; }
        public string Message { get; set; }
        public bool Hide { get; set; }
        public bool OnlyRibbon { get; set; }
        public byte SortOrder { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as GroupMedal;
			if (t == null) return false;
			if (GroupID == t.GroupID
			 && MedalID == t.MedalID)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ GroupID.GetHashCode();
			hash = (hash * 397) ^ MedalID.GetHashCode();

			return hash;
        }
        #endregion
    }
}
