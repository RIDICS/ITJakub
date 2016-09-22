using System;
using System.Reflection;

namespace ITJakub.Web.Hub.Managers
{
    public static class VersionInfo
    {
        private static string m_version;

        static VersionInfo()
        {
            
        }

        public static string Version
        {
            get
            {
                var version =
                Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyInformationalVersionAttribute)) as
                    AssemblyInformationalVersionAttribute;

                m_version = version != null
                    ? version.InformationalVersion
                    : Assembly.GetExecutingAssembly().GetName().Version.ToString();

                return m_version;
            }
        }
    }
}