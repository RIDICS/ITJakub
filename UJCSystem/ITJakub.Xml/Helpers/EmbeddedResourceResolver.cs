using System;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;

namespace ITJakub.Xml.Helpers
{
    public class EmbeddedResourceResolver : XmlUrlResolver
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public EmbeddedResourceResolver(Type xsltType)
        {
            XsltType = xsltType;
        }

        public Type XsltType { get; private set; }
        public override object GetEntity(Uri absoluteUri,string role, Type ofObjectToReturn)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Getting XSLT Transformation from Embedded resource: {0}", Path.GetFileName(absoluteUri.AbsolutePath));

            return assembly.GetManifestResourceStream(XsltType, Path.GetFileName(absoluteUri.AbsolutePath));
        }
    }
}