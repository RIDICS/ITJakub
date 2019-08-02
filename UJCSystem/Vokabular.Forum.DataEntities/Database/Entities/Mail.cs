using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Mail {
        public int MailID { get; set; }
        public string FromUser { get; set; }
        public string FromUserName { get; set; }
        public string ToUser { get; set; }
        public string ToUserName { get; set; }
        public DateTime Created { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string BodyHtml { get; set; }
        public int SendTries { get; set; }
        public DateTime? SendAttempt { get; set; }
        public int? ProcessID { get; set; }
    }
}
