using System;

namespace ITJakub.MobileApps.DataContracts
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizedMethodAttribute : Attribute
    {
        private readonly string m_userIdParameterName;
        private readonly UserRoleContract m_userRoleContract;

        public AuthorizedMethodAttribute(UserRoleContract minRoleContractAllowed, string userIdParameterName=null)
        {
            m_userIdParameterName = userIdParameterName;
            m_userRoleContract = minRoleContractAllowed;
        }

        public UserRoleContract MinRoleContractAllowed
        {
            get { return m_userRoleContract; }
        }

        public string UserIdParameterName
        {
            get { return m_userIdParameterName; }
        }
    }
}