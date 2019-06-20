using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts.Permission
{
    public class RoleContract
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class RoleDetailContract : RoleContract
    {
        public List<PermissionContract> Permissions { get; set; }
    }
}