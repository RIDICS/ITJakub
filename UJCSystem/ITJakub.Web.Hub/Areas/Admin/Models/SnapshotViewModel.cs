using System;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class SnapshotViewModel
    {
        public long Id { get; set; }
        public DateTime PublishDate { get; set; }
        public int TextResourceCount { get; set; }
        public int PublishedTextResourceCount { get; set; }
        public int ImageResourceCount { get; set; }
        public int PublishedImageResourceCount { get; set; }
        public int AudioResourceCount { get; set; }
        public int PublishedAudioResourceCount { get; set; }
        public int VideoResourceCount { get; set; }
        public int PublishedVideoResourceCount { get; set; }
        public string Author { get; set; }
    }
}
