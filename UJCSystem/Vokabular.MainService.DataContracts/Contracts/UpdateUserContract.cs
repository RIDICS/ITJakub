using Ridics.Authentication.DataContracts;

namespace Vokabular.MainService.DataContracts.Contracts
{
    public class UpdateUserContract
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AvatarUrl { get; set; }
    }

    public class UpdateUserPasswordContract
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }

    public class UserContactContract
    {
        public ContactTypeEnum ContactType { get; set; }
    }

    public class UpdateUserContactContract : UserContactContract
    {
        public string NewContactValue { get; set; }

        public string OldContactValue { get; set; }
    }

    public class ConfirmUserContactContract : UserContactContract
    {
        public string ConfirmCode { get; set; }
    }

    public class UpdateTwoFactorContract
    {
        public bool TwoFactorIsEnabled { get; set; }

        public string TwoFactorProvider { get; set; }
    }
}