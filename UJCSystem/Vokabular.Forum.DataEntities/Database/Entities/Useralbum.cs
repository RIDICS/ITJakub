using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class UserAlbum {
        public UserAlbum() {
			UserAlbumImages = new List<UserAlbumImage>();
        }
        public int AlbumID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public int? CoverImageID { get; set; }
        public DateTime Updated { get; set; }
        public IList<UserAlbumImage> UserAlbumImages { get; set; }
    }
}
