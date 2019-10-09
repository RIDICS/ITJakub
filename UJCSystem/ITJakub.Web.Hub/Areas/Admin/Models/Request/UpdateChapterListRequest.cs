using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.Models.Request
{
    public class UpdateChapterListRequest
    {
        public long ProjectId { get; set; }
        public IList<UpdateChapterRequest> ChapterList { get; set; }
    }
    
    public class UpdateChapterRequest
    {
        public string Comment { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public long BeginningPageId { get; set; }
        public long? ParentChapterId { get; set; }
        public long? Id { get; set; }
    }
}
