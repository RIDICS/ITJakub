using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class CheckEmail {
        public int CheckEmailID { get; set; }
        public User User { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public string Hash { get; set; }
    }
}
