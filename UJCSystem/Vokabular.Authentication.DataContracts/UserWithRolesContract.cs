using System.Collections.Generic;

namespace Vokabular.Authentication.DataContracts
{
    public class UserWithRolesContract : ContractBase
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<RoleContract> Roles { get; set; }
    }
}