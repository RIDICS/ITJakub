using System;
using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class SnapshotContract
    {
        public long Id { get; set; }
        public int VersionNumber { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime PublishTime { get; set; }
        public string Comment { get; set; }

        public long ProjectId { get; set; }
        public int CreatedByUserId { get; set; }
        public BookTypeEnumContract DefaultBookType { get; set; }
        //public BookVersionResource BookVersion { get; set; }

        public IList<BookTypeEnumContract> BookTypes { get; set; }
    }

    public class CreateSnapshotContract
    {
        public IList<long> ResourceVersionIds { get; set; }
        public string Comment { get; set; }
        public IList<BookTypeEnumContract> BookTypes { get; set; }
        public BookTypeEnumContract DefaultBookType { get; set; }
    }
}