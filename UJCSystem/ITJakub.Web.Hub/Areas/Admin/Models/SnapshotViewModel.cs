using System;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class SnapshotViewModel
    {
        public long Id { get; set; }
        public DateTime PublishDate { get; set; }
        public int PublishedTextResourceCount { get; set; }
        public int PublishedImageResourceCount { get; set; }
        public int PublishedAudioResourceCount { get; set; }
        public string Author { get; set; }
    }
}
