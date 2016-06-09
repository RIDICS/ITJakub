using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Xml;
using Daliboris.Pomucky.Xml;
using Daliboris.Texty.Export;
using Daliboris.Texty.Export.Rozhrani;
using Ujc.Ovj.ChangeEngine.Objects;
using Ujc.Ovj.Tools.Xml.XsltTransformation;

namespace Daliboris.Slovniky
{
    public class DDBW : JgSlov
    {
        public DDBW()
        {
            m_changeRuleSetFile = "Daliboris.Slovniky.Xmr.DDBW.xmr";
        }

        public override void SeskupitHeslaPismene(string inputFile, string outputFile, string filenameWithoutExtension)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var changeRuleSet = ChangeRuleSet.Load(assembly.GetManifestResourceStream(m_changeRuleSetFile));

            var xws = new XmlWriterSettings {Indent = true};

            using (var xmlWriter = XmlWriter.Create(outputFile, xws))
            using (var xmlReader = XmlReader.Create(inputFile))
            {
                xmlWriter.WriteStartDocument();

                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (xmlReader.Name)
                            {
                                case "reg":
                                    if (xmlReader.GetAttribute("xml:compute-reg") != "true")
                                    {
                                        Transformace.SerializeNode(xmlReader, xmlWriter);
                                    }
                                    else
                                    {
                                        xmlWriter.WriteStartElement(xmlReader.Name);

                                        while (xmlReader.MoveToNextAttribute())
                                        {
                                            if (xmlReader.Name == "xml:compute-reg")
                                            {
                                                continue;
                                            }

                                            xmlWriter.WriteAttributeString(xmlReader.Prefix, xmlReader.LocalName, xmlReader.NamespaceURI, xmlReader.Value);
                                        }

                                        xmlReader.MoveToContent();

                                        xmlWriter.WriteString(changeRuleSet.Apply(xmlReader.ReadInnerXml()));

                                        xmlWriter.WriteEndElement();
                                    }

                                    break;
                                default:
                                    Transformace.SerializeNode(xmlReader, xmlWriter);

                                    break;
                            }

                            break;
                        case XmlNodeType.EndElement:
                            switch (xmlReader.Name)
                            {
                                case "teiHeader":
                                    Transformace.SerializeNode(xmlReader, xmlWriter);

                                    break;
                                default:
                                    Transformace.SerializeNode(xmlReader, xmlWriter);

                                    break;
                            }

                            break;
                        default:
                            Transformace.SerializeNode(xmlReader, xmlWriter);

                            break;
                    }
                }

                xmlWriter.WriteEndDocument();
            }
        }

        public override void GenerateConversionMetadataFile(
            ExportBase export,
            IExportNastaveni settings,
            string documentType,
            string finalOutputFileFullPath,
            string finalOutputFileName,
            string finalOutputMetadataFileName)
        {
            var fiFinalOutputFilename = new FileInfo(finalOutputFileFullPath);
            var step = 0;
            var outputFileWithoutExtension = fiFinalOutputFilename.Name.Substring(0, fiFinalOutputFilename.Name.LastIndexOf(".", StringComparison.Ordinal));

            var fileTransformationSource = finalOutputFileFullPath;

            var fileTransformationTarget = GetTempFile(settings.DocasnaSlozka, outputFileWithoutExtension, step++);
            TestExtrahujHesla(fileTransformationSource, fileTransformationTarget, outputFileWithoutExtension, m_changeRuleSetFile, false);
            //fileTransformationSource = fileTransformationTarget;

            var parameters = new NameValueCollection
            {
                {"accessories", finalOutputFileName},
                {"fascimileDoc", fileTransformationTarget}
            };

            foreach (var transformationFile in XsltTransformerFactory.GetTransformationFromTransformationsFile(settings.SouborTransformaci, "ddbw-xmd-step"))
            {
                fileTransformationTarget = GetTempFile(settings.DocasnaSlozka, outputFileWithoutExtension, step++);

                export.ApplyTransformations(fileTransformationSource, fileTransformationTarget, XsltTransformerFactory.GetXsltTransformers(
                    settings.SouborTransformaci,
                    transformationFile,
                    settings.SlozkaXslt, true), settings.DocasnaSlozka, parameters);

                fileTransformationSource = fileTransformationTarget;
            }

            File.Copy(fileTransformationSource, finalOutputMetadataFileName);
        }
    }
}
