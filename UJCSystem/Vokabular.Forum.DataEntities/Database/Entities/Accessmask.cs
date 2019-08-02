using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class AccessMask {
        public AccessMask() {
			ForumAccesses = new List<ForumAccess>();
			UserForums = new List<UserForum>();
        }
        public int AccessMaskID { get; set; }
        public Board Board { get; set; }
        public string Name { get; set; }
        public int Flags { get; set; }
        public short SortOrder { get; set; }
        public IList<ForumAccess> ForumAccesses { get; set; }
        public IList<UserForum> UserForums { get; set; }
    }
}
