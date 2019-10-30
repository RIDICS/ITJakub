using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.Admin.Models
{
    public class ProjectListViewModel
    {
        public ListViewModel<ProjectItemViewModel> Projects { get; set; }
        public IList<BookTypeEnumContract> AvailableBookTypes { get; set; }
        public IList<ProjectOwnerType> FilterTypes { get; set; }
    }
}