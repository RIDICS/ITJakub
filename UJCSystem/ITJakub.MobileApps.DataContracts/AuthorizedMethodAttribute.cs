using System;

namespace ITJakub.MobileApps.DataContracts
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizedMethodAttribute : Attribute
    {
        private readonly string m_userIdParameterName;
        private readonly Role m_role;

        public AuthorizedMethodAttribute(Role minRoleAllowed, string userIdParameterName=null)
        {
            m_userIdParameterName = userIdParameterName;
            m_role = minRoleAllowed;
        }

        public Role MinRoleAllowed
        {
            get { return m_role; }
        }

        public string UserIdParameterName
        {
            get { return m_userIdParameterName; }
        }
    }

    public enum Role
    {
        Student,
        Teacher
    }
}