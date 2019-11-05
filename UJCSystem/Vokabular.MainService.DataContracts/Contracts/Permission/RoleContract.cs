using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.DataContracts.Contracts.Permission
{
    public class RoleContract
    {
        public int Id { get; set; }

        public int ExternalId { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class UserGroupContract : RoleContract
    {
        public UserGroupTypeContract Type { get; set; }
    }

    public class RoleDetailContract : UserGroupContract
    {
        public List<SpecialPermissionContract> Permissions { get; set; }
    }
}