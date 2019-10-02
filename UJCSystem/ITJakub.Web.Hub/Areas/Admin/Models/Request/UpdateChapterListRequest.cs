using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Areas.Admin.Models.Request
{
    public class UpdateChapterListRequest
    {
        public long ProjectId { get; set; }
        public IList<CreateOrUpdateChapterContract> ChapterList { get; set; }
    }
}
