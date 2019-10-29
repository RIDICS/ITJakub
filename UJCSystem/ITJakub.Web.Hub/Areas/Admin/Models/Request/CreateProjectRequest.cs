using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.Admin.Models.Request
{
    public class CreateProjectRequest
    {
        public string Name { get; set; }
        
        public IList<BookTypeEnumContract> SelectedBookTypes { get; set; }
    }
}
