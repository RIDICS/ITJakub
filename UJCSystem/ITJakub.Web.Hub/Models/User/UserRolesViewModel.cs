using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace ITJakub.Web.Hub.Models.User
{
    public class UserRolesViewModel
    {
        public IList<RoleContract> Roles { get; set; }
    }
}
