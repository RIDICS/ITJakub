using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vokabular.Authentication.DataContracts
{
    public class PermissionContract : ContractBase
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<RoleContract> Roles { get; set; }
    }
}
