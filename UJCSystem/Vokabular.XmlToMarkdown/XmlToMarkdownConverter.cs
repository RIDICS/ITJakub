using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;
using Vokabular.Shared.Converters;

namespace Vokabular.XmlToMarkdown
{
    public class XmlToMarkdownConverter : IXmlToTextConverter
    {
        private const string TransformationFile = "Vokabular.XmlToMarkdown.XslTransformations.pageToPlainText.xsl";

        public string Convert(Stream stream)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var xslstream = assembly.GetManifestResourceStream(TransformationFile) ?? throw new InvalidOperationException();

            XslCompiledTransform xslTrans = new XslCompiledTransform();
            string text;
            using (xslstream)
            {
                using (var xmlReader = XmlReader.Create(xslstream))
                {
                    xslTrans.Load(xmlReader);
                    using (Stream memStream = new MemoryStream())
                    {
                        using (XmlReader xml = XmlReader.Create(stream))
                        {
                            xslTrans.Transform(xml, null, memStream);
                        }

                        memStream.Seek(0, SeekOrigin.Begin);
                        using (StreamReader reader = new StreamReader(memStream))
                        {
                            text = reader.ReadToEnd();
                        }
                    }
                };
            }

            return text;
        }
    }
}
