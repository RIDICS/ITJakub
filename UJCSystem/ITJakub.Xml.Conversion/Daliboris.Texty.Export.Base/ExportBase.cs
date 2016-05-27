using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Daliboris.Texty.Evidence.Rozhrani;
using Daliboris.Texty.Export.Rozhrani;
using ITJakub.Shared.Contracts.Resources;
using NLog;
using Ujc.Ovj.Tools.Xml.XsltTransformation;

namespace Daliboris.Texty.Export
{
    public abstract class ExportBase
    {
        public bool UsePersonalizedXmdGenerator { get; protected set; } = false;

        protected IExportNastaveni Nastaveni { get; }

        protected ExportBase(IExportNastaveni nastaveni)
        {
            Nastaveni = nastaveni;
        }

        public virtual void Exportuj(IPrepis prpPrepis, IList<string> xmlOutputFiles, Dictionary<ResourceType, string[]> uploadedFiles)
        {
            Exportuj(prpPrepis, xmlOutputFiles);
        }

        protected abstract void Exportuj(IPrepis prpPrepisy, IList<string> xmlOutputFiles);

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        protected void Zaloguj(string zprava)
        {
            Zaloguj(zprava, false);
        }

        protected void Zaloguj(string format, object arg0, bool chyba)
        {
            object[] args = new object[1];
            args[0] = arg0;
            Zaloguj(format, args, chyba);
        }

        protected void Zaloguj(string format, object arg0, object arg1, bool chyba)
        {
            object[] args = new object[2];
            args[0] = arg0;
            args[1] = arg1;
            Zaloguj(format, args, chyba);

        }

        protected void Zaloguj(string format, object arg0, object arg1, object arg2, bool chyba)
        {
            object[] args = new object[1];
            args[0] = arg0;
            args[1] = arg1;
            args[2] = arg2;
            Zaloguj(format, args, chyba);
        }

        protected void Zaloguj(string format, object[] args, bool chyba)
        {
            Zaloguj(String.Format(format, args), chyba);
        }


        protected void Zaloguj(string zprava, bool chyba)
        {
            ConsoleColor backgroundColor = Console.BackgroundColor;
            ConsoleColor foregroundColor = Console.ForegroundColor;
            if (!Nastaveni.VypisovatVse && !chyba) return;
            if (chyba)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(zprava);
            }
            _logger.Debug(zprava);

            if (chyba)
            {
                Console.ForegroundColor = foregroundColor;
                Console.BackgroundColor = backgroundColor;
            }

        }

        protected Queue<IList<IXsltTransformer>> GetTransformationList(string transformationPrefix)
        {
            var xsltSteps = new Queue<IList<IXsltTransformer>>();

            foreach (var transformationFile in XsltTransformerFactory.GetTransformationFromTransformationsFile(Nastaveni.SouborTransformaci, transformationPrefix))
            {
                xsltSteps.Enqueue(
                    XsltTransformerFactory.GetXsltTransformers(
                        Nastaveni.SouborTransformaci,
                        transformationFile,
                        Nastaveni.SlozkaXslt, true));
            }

            return xsltSteps;
        }

        protected string GetTempFile(string tempDirectory, string sourceFile, int step)
        {
            const string fileNameFormat = "{0}_{1:00}.xml";

            return Path.Combine(tempDirectory, String.Format(fileNameFormat, sourceFile, step));
        }

        public void ApplyTransformations(string inputFile, string outputFile, IList<IXsltTransformer> transformers, string tempDirectory, NameValueCollection parameters = null)
        {
            var process = new XsltTransformationProcess(inputFile, outputFile, transformers, parameters ?? new NameValueCollection());
            process.TempDirectory = tempDirectory;
            process.Transform();
        }

        public virtual void GenerateConversionMetadataFile(
            string documentType, 
            string finalOutputFileFullPath, 
            string finalOutputFileName, 
            string finalOutputMetadataFileName) {
            throw new NotImplementedException();
        }
    }
}
