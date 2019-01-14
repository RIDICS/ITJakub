using System;

namespace Vokabular.Authentication.Structures
{
    public interface IConvertableToUserData
    {
        string Title { get; set; }

        string Prefix { get; set; }

        string SecondName { get; set; }

        string FullName { get; set; }

        string Suffix { get; set; }

        DateTime BirthTime { get; set; }

        string PersonalIdentificationNumber { get; set; }

        string InsuranceNumber { get; set; }

        string InsuranceCompanyCode { get; set; }

        string EhicCode { get; set; }

        string IdentityCardCode { get; set; }

        string PassportCountry { get; set; }

        string PassportNumber { get; set; }
    }
}
