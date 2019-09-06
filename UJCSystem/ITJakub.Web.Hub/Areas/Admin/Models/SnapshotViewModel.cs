using System;
using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Types;

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
        public string Comment { get; set; }
    }

    public class CreateSnapshotViewModel
    {
        public long ProjectId { get; set; }
        public string Comment { get; set; }
        public IList<long> ResourceVersionIds { get; set; }
        public IList<BookTypeEnumContract> BookTypes { get; set; }
        public BookTypeEnumContract DefaultBookType { get; set; }
    }
}
