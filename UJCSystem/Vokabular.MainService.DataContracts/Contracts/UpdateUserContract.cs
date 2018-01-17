namespace Vokabular.MainService.DataContracts.Contracts
{
    public class UpdateUserContract
    {
        //public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AvatarUrl { get; set; }

        public string Email { get; set; }
    }

    public class UpdateUserPasswordContract
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}