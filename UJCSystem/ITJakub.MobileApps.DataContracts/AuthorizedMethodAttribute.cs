using System;

namespace ITJakub.MobileApps.DataContracts
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizedMethodAttribute : Attribute
    {
        private readonly string m_userIdParameterName;
        private readonly UserRole m_userRole;

        public AuthorizedMethodAttribute(UserRole minRoleAllowed, string userIdParameterName=null)
        {
            m_userIdParameterName = userIdParameterName;
            m_userRole = minRoleAllowed;
        }

        public UserRole MinRoleAllowed
        {
            get { return m_userRole; }
        }

        public string UserIdParameterName
        {
            get { return m_userIdParameterName; }
        }
    }
}