using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class FavoriteTopic {
        public int ID { get; set; }
        public User User { get; set; }
        public Topic Topic { get; set; }
    }
}
