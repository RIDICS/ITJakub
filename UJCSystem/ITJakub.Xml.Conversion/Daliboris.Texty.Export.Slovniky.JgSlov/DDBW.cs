using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using Daliboris.Texty.Export;
using Daliboris.Texty.Export.Rozhrani;
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
            File.Copy(inputFile, outputFile);
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