namespace Vokabular.Authentication.Structures
{
    public interface IConvertableToUserContacts
    {
        string Email { get; set; }

        bool EmailConfirmed { get; set; }

        string EmailConfirmCode { get; set; }

        string PhoneNumber { get; set; }

        bool PhoneNumberConfirmed { get; set; }

        string PhoneNumberConfirmCode { get; set; }
    }
}