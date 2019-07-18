using System.Collections.Generic;
using Newtonsoft.Json;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class UserContract
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [JsonIgnore]
        public int ExternalId { get; set; }
    }

    public class UserDetailContract : UserContract
    {
        public string Email { get; set; }

        public List<RoleContract> Roles { get; set; }
    }

    public class CreateUserContract : UserContract
    {
        public string Email { get; set; }

        public string NewPassword { get; set; }
    }
}
