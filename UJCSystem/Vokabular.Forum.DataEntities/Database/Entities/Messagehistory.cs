using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class MessageHistory {
        public int MessageID { get; set; }
        public DateTime Edited { get; set; }
        public Message Message { get; set; }
        public string MessageText { get; set; }
        public string IP { get; set; }
        public int? EditedBy { get; set; }
        public string EditReason { get; set; }
        public bool IsModeratorChanged { get; set; }
        public int Flags { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as MessageHistory;
			if (t == null) return false;
			if (MessageID == t.MessageID
			 && Edited == t.Edited)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ MessageID.GetHashCode();
			hash = (hash * 397) ^ Edited.GetHashCode();

			return hash;
        }
        #endregion
    }
}
