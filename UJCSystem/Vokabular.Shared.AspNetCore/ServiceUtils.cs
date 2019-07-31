using System;
using System.IO;
using System.Reflection;

namespace Vokabular.Shared.AspNetCore
{
    public static class ServiceUtils
    {
        public static string GetAppXmlDocumentationPath(Type typeFromAssembly = null)
        {
            var assembly = typeFromAssembly != null ? typeFromAssembly.Assembly : Assembly.GetEntryAssembly();
            return GetAppXmlDocumentationPath(assembly);
        }

        public static string GetAppXmlDocumentationPath(Assembly assembly)
        {
            var appBasePath = AppContext.BaseDirectory;
            var appName = assembly.GetName().Name;
            return Path.Combine(appBasePath, $"{appName}.xml");
        }
    }
}
