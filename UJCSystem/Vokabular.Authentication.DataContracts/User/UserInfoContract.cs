namespace Vokabular.Authentication.DataContracts.User
{
    public class UserInfoContract
    {
        public string UserName { get; set; }

        public string VerificationCode { get; set; }

        public int UserId { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}