using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;
using Vokabular.Shared.Converters;

namespace Vokabular.XmlToMarkdown
{
    public class XmlToMarkdownConverter : IXmlToTextConverter
    {
        private const string TransformationFile = "\\Vokabular.XmlToMarkdown\\XslTransformations\\pageToHtml.xsl";
        public string Convert(Stream stream)
        {
            /*Assembly assembly = Assembly.GetExecutingAssembly();
            var xslt = assembly.GetFile("Vokabular.XmlToMarkdown.XslTransformations.pageToHtml.xsl");
            XmlReader xmlReader = new XmlTextReader(xslt);*/ //TODO 
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;//HACK to build absolute path to xsl transformation file
            DirectoryInfo parentDir = Directory.GetParent(currentDirectory.EndsWith("\\") ? currentDirectory : string.Concat(currentDirectory, "\\"));
            var myParentDir = parentDir.Parent.FullName;
            var transformationFilePath = $"{myParentDir}{TransformationFile}";

            XslCompiledTransform myXslTrans = new XslCompiledTransform();
            myXslTrans.Load(transformationFilePath);
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
            markdown = Regex.Replace(markdown, "</h.*?>", Environment.NewLine);
            foreach (Match match in Regex.Matches(markdown, "class=\"itj-line\""))
            {
                var index = match.Index;
                Stack<int> elements = new Stack<int>();
                elements.Push(index);
                while (elements.Count != 0)
                {
                    var mat = Regex.Match(markdown.Substring(elements.Peek()), "<([^>]*span[^>]*)>");
                    var m = mat.Groups[1].Value.Split(' ')[0];
                    if (m[0].Equals('/'))
                    {
                        index = elements.Pop();
                        
                    } else
                    {
                        elements.Push(match.Index + mat.Index + 1);
                    }
                }
                var tm = Regex.Match(markdown.Substring(index), ">");
                markdown.Insert(tm.Index + 1, Environment.NewLine);

            }
            return Regex.Replace(markdown, "<.*?>", string.Empty);
        }
    }
}
