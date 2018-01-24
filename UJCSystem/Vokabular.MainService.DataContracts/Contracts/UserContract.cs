using System;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class UserContract
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AvatarUrl { get; set; }
    }

    public class UserDetailContract : UserContract
    {
        public string Email { get; set; }

        public DateTime CreateTime { get; set; }
    }

    public class CreateUserContract : UserContract
    {
        public string Email { get; set; }

        public string NewPassword { get; set; }
    }
}
