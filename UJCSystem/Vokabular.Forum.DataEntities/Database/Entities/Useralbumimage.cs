using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class UserAlbumImage {
        public int ImageID { get; set; }
        public UserAlbum UserAlbum { get; set; }
        public string Caption { get; set; }
        public string FileName { get; set; }
        public int Bytes { get; set; }
        public string ContentType { get; set; }
        public DateTime Uploaded { get; set; }
        public int Downloads { get; set; }
    }
}
