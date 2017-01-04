using System;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ProjectContract
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastEditDate { get; set; }
        public UserContract CreateUser { get; set; }
        public UserContract LastEditUser { get; set; }
        public string PublisherText { get; set; }
        public string LiteraryOriginalText { get; set; }
        public int PageCount { get; set; }
    }
}
