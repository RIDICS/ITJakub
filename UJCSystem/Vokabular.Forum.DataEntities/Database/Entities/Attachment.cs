using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class Attachment {
        public int AttachmentID { get; set; }
        public User User { get; set; }
        public int MessageID { get; set; }
        public string FileName { get; set; }
        public int Bytes { get; set; }
        public string ContentType { get; set; }
        public int Downloads { get; set; }
        public byte[] FileData { get; set; }
    }
}
