using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vokabular.Authentication.DataContracts
{
    public class RoleContractBase : ContractBase
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class RoleContract : RoleContractBase
    {
        public List<PermissionContract> Permissions { get; set; }
    }
}
