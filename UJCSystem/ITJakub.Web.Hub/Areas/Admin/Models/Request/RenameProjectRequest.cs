using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.Admin.Models.Request
{
    public class RenameProjectRequest
    {
        public long Id { get; set; }
        
        public string NewProjectName { get; set; }
    }
}
