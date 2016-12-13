using System;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ResourceVersionContract
    {
        public long Id { get; set; }
        public int VersionNumber { get; set; }
        public string Comment { get; set; }
        public DateTime CreateDate { get; set; }
        public string Author { get; set; }
    }
}