using System;

namespace ITJakub.MobileApps.DataContracts
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizedMethodAttribute : Attribute
    {
        public readonly Role m_role;

        public AuthorizedMethodAttribute(Role minRoleAllowed)
        {
            m_role = minRoleAllowed;
        }

        public Role MinRoleAllowed
        {
            get { return m_role; }
        }
    }

    public enum Role
    {
        Student,
        Teacher
    }
}