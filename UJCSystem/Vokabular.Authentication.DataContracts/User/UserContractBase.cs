using System;
using System.Collections.Generic;
using Vokabular.Authentication.DataContracts.SharedStructures;

namespace Vokabular.Authentication.DataContracts.User
{
    public class UserContractBase : ContractBase
    {
        public int Id { get; set; }

        public Guid? MasterUserId { get; set; }

        public UserAddressingWays Title { get; set; }

        public string Prefix { get; set; }
 
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string SecondName { get; set; }
        
        public string FullName { get; set; }

        public string Suffix { get; set; }

        public DateTime BirthTime { get; set; }

        public string PhoneNumber { get; set; }

        public ContactLevelOfAssuranceEnum PhoneLevelOfAssurance { get; set; }

        public string Email { get; set; }

        public ContactLevelOfAssuranceEnum EmailLevelOfAssurance { get; set; }
        
        public string PhoneNumberConfirmCode { get; set; }
        
        public string EmailConfirmCode { get; set; }

        public string PersonalIdentificationNumber { get; set; }

        public string InsuranceCompanyCode { get; set; }

        public string InsuranceNumber { get; set; }

        public string EhicCode { get; set; }

        public string IdentityCardCode { get; set; }

        public string PassportCountry { get; set; }

        public string PassportNumber { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public string TwoFactorProvider { get; set; }

        public IList<string> ValidTwoFactorProviders { get; set; }

        public List<RoleContract> Roles { get; set; }
        
        public List<ExternalLoginContract> ExternalLogin { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool PhoneNumberConfirmed { get; set; }
    }
}