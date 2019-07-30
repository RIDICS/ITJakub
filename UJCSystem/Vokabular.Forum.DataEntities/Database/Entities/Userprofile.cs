using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class UserProfile {
        public int UserID { get; set; }
        public string ApplicationName { get; set; }
        public User User { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public DateTime? LastActivity { get; set; }
        public bool IsAnonymous { get; set; }
        public string UserName { get; set; }
        public int? Gender { get; set; }
        public string Blog { get; set; }
        public string RealName { get; set; }
        public string Interests { get; set; }
        public string Skype { get; set; }
        public string Facebook { get; set; }
        public string Location { get; set; }
        public string BlogServiceUrl { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? LastSyncedWithDNN { get; set; }
        public string ICQ { get; set; }
        public string City { get; set; }
        public string MSN { get; set; }
        public string TwitterId { get; set; }
        public string Twitter { get; set; }
        public string BlogServicePassword { get; set; }
        public string Country { get; set; }
        public string Occupation { get; set; }
        public string Region { get; set; }
        public string AIM { get; set; }
        public string XMPP { get; set; }
        public string YIM { get; set; }
        public string Google { get; set; }
        public string BlogServiceUsername { get; set; }
        public string GoogleId { get; set; }
        public string Homepage { get; set; }
        public string FacebookId { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as UserProfile;
			if (t == null) return false;
			if (UserID == t.UserID
			 && ApplicationName == t.ApplicationName)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ UserID.GetHashCode();
			hash = (hash * 397) ^ ApplicationName.GetHashCode();

			return hash;
        }
        #endregion
    }
}
