using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Rank {
        public Rank() {
			Users = new List<User>();
        }
        public int RankID { get; set; }
        public Board Board { get; set; }
        public string Name { get; set; }
        public int? MinPosts { get; set; }
        public string RankImage { get; set; }
        public int Flags { get; set; }
        public int? PMLimit { get; set; }
        public string Style { get; set; }
        public short SortOrder { get; set; }
        public string Description { get; set; }
        public int UsrSigChars { get; set; }
        public string UsrSigBBCodes { get; set; }
        public string UsrSigHTMLTags { get; set; }
        public int UsrAlbums { get; set; }
        public int UsrAlbumImages { get; set; }
        public IList<User> Users { get; set; }
    }
}
