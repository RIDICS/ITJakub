using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;
using Vokabular.Shared.Converters;

namespace Vokabular.XmlToMarkdown
{
    public class XmlToMarkdownConverter : IXmlToTextConverter
    {
        private const string transformationFile = "C:\\Pool\\itjakub\\Database\\ExistDB\\transformations\\pageToHtml.xsl";
        public string Convert(Stream stream)
        {
            XslCompiledTransform myXslTrans = new XslCompiledTransform();
            myXslTrans.Load(transformationFile);
            string text;
            using (Stream memStream = new MemoryStream())
            {
                using (XmlReader xml = XmlReader.Create(stream))
                {
                    myXslTrans.Transform(xml, null, memStream);
                }

                memStream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(memStream))
                {
                    text = reader.ReadToEnd();
                }
            }
            var markdown = Regex.Replace(text, "<h.*?>", "## ");
            return Regex.Replace(markdown, "<.*?>", string.Empty);
        }
    }
}
