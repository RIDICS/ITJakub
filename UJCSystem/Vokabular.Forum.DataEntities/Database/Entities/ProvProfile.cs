using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class ProvProfile {
        public string UserID { get; set; }
        public DateTime LastUpdatedDate { get; set; }
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
    }
}
