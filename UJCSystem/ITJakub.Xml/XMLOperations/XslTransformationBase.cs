﻿using System.IO;
using System.Xml;
using System.Xml.Xsl;
using ITJakub.Xml.Helpers;

namespace ITJakub.Xml.XMLOperations
{
    public abstract class XslTransformationBase
    {
        public XslTransformationBase()
        {
            InnerTransformation = LoadTransformation();
        }

        public abstract string SourceFile { get; }
        protected XslCompiledTransform InnerTransformation { get; private set; }


        public string Transform(string xml, string searchedTerm)
        {
            var arguments = GetArgumentList(searchedTerm);
            StringWriter writer = new StringWriter();
            XmlReader xmlReadB = new XmlTextReader(new StringReader(xml));

            InnerTransformation.Transform(xmlReadB, arguments, writer);

            return writer.ToString();
        }

        private XsltArgumentList GetArgumentList(string searchedTerm)
        {
            XsltArgumentList argsList = new XsltArgumentList();
            argsList.AddParam("searchedLemma", "", searchedTerm);

            return argsList;
        }

        protected XslCompiledTransform LoadTransformation()
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            EmbeddedResourceResolver resolver = new EmbeddedResourceResolver(GetType());
            xslt.Load(SourceFile, XsltSettings.TrustedXslt, resolver);

            return xslt;
        }

        //protected XslCompiledTransform LoadTransformation()
        //{


        //    XslCompiledTransform xslt = new XslCompiledTransform();
        //    EmbeddedResourceResolver resolver = new EmbeddedResourceResolver();
        //    xslt.Load(SourceFile, XsltSettings.TrustedXslt, resolver);


        //    XmlReader xsltReader = XmlReader.Create(GetType().Assembly.GetManifestResourceStream(SourceFile));           
        //    XslCompiledTransform result = new XslCompiledTransform(true);
        //    result.Load(xsltReader);
        //    return result;
        //}
    }
}