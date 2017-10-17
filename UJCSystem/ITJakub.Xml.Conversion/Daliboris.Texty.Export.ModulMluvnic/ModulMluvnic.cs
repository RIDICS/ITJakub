using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using CodeCarvings.Piczard.Filters.Watermarks;
using Daliboris.Pomucky.Xml;
using Daliboris.Texty.Evidence.Rozhrani;
using Daliboris.Texty.Export.Rozhrani;
using Vokabular.Shared.DataContracts.Types;

namespace Daliboris.Texty.Export
{
    public class ModulMluvnic : ExportBase
    {
        public ModulMluvnic(IExportNastaveni nastaveni) : base(nastaveni)
        {
            UsePersonalizedXmdGenerator = true;
        }

        protected override void Exportuj(IPrepis prpPrepis, IList<string> xmlOutputFiles)
        {
            throw new NotImplementedException();
        }

        public override void Exportuj(IPrepis prpPrepis, IList<string> xmlOutputFiles, IReadOnlyDictionary<ResourceType, string[]> uploadedFiles)
        {
            ExportujImpl(prpPrepis, xmlOutputFiles, uploadedFiles);
        }

        public override void GenerateConversionMetadataFile(
            string documentType,
            string finalOutputFileFullPath,
            string finalOutputFileName,
            string finalOutputMetadataFileName)
        {
            var fiFinalOutputFilename = new FileInfo(finalOutputFileFullPath);
            var outputFileWithoutExtension = fiFinalOutputFilename.Name.Substring(0, fiFinalOutputFilename.Name.LastIndexOf(".", StringComparison.Ordinal)) + "-xmd";

            var xsltSteps = GetTransformationList("grammar-xmd-step");
            var stepFiles = new string[xsltSteps.Count];
            for (var i = 0; i < stepFiles.Length; i++)
            {
                stepFiles[i] = GetTempFile(Nastaveni.DocasnaSlozka, outputFileWithoutExtension, i);
            }

            var guid = Guid.NewGuid();

            var parameters = new NameValueCollection
            {
                {"versionId", guid.ToString("D")}
            };

            var step = 0;
            var inputFilePath = finalOutputFileFullPath;
            while (xsltSteps.Count > 0)
            {
                ApplyTransformations(inputFilePath, stepFiles[step], xsltSteps.Dequeue(), Nastaveni.DocasnaSlozka, parameters);
                inputFilePath = stepFiles[step++];
            }

            File.Copy(inputFilePath, finalOutputMetadataFileName);
        }

        private void ExportujImpl(IPrepis prepis, IList<string> xmlOutputFiles, IReadOnlyDictionary<ResourceType, string[]> uploadedFiles)
        {
            var xsltSteps = GetTransformationList("grammar-step");

            var fileNameFithoutExtension = prepis.Soubor.NazevBezPripony;
            var fullFileName = fileNameFithoutExtension + ".xml";

            var inputFilePath = Path.Combine(Nastaveni.VstupniSlozka, fullFileName);

            var faksimile = uploadedFiles[ResourceType.UnknownXmlFile].First(xmlFile =>
            {
                var xmlFileInfo = new FileInfo(xmlFile);

                return xmlFileInfo.Name.Substring(0, xmlFileInfo.Name.Length - xmlFileInfo.Extension.Length) == fileNameFithoutExtension;
            });

            var stepFiles = new string[xsltSteps.Count + 4]; //UpravPerexSouboru, ZkontrolujCislaPagin, OdlisitStejnePojmenovanaPaginy, UpravOdkazyNaPaginy
            for (var i = 0; i < stepFiles.Length; i++)
            {
                stepFiles[i] = GetTempFile(Nastaveni.DocasnaSlozka, fileNameFithoutExtension, i);
            }

            var parameters = new NameValueCollection {{"guid", prepis.GUID}, {"faksimile", faksimile}};
            ApplyTransformations(inputFilePath, stepFiles[0], xsltSteps.Dequeue(), Nastaveni.DocasnaSlozka, parameters);

            ApplyTransformations(stepFiles[0], stepFiles[1], xsltSteps.Dequeue(), Nastaveni.DocasnaSlozka, parameters);

            ApplyTransformations(stepFiles[1], stepFiles[2], xsltSteps.Dequeue(), Nastaveni.DocasnaSlozka);

            UpravPerexSouboru(new FileInfo(stepFiles[2]), stepFiles[3]);

            ZkontrolujCislaPagin(new FileInfo(stepFiles[3]), stepFiles[4]);
            OdlisitStejnePojmenovanaPaginy(new FileInfo(stepFiles[4]), stepFiles[5]);
            UpravOdkazyNaPaginy(new FileInfo(stepFiles[5]), stepFiles[6]);

            AppendWatermarks(new FileInfo(stepFiles[6]), uploadedFiles[ResourceType.Image]);

            var outputXml = Path.Combine(Nastaveni.VystupniSlozka, fullFileName);

            File.Copy(stepFiles[6], outputXml);

            /*
            IList<IXsltTransformer> body = XsltTransformerFactory.GetXsltTransformers(Nastaveni.SouborTransformaci, "body", Nastaveni.SlozkaXslt);
            string konecnyVystup = null;
            
            const string csPriponaXml = ".xml";

            DateTime casExportu = Nastaveni.CasExportu == DateTime.MinValue ? DateTime.Now : Nastaveni.CasExportu;
            string souborBezPripony = prepis.Soubor.NazevBezPripony;

            konecnyVystup = Path.Combine(Nastaveni.VystupniSlozka, prepis.Soubor.NazevBezPripony + csPriponaXml);

            string headerFile = Path.Combine(Nastaveni.DocasnaSlozka, String.Format("{0}_{1}.xml", souborBezPripony, "header"));
            NameValueCollection parameters = new NameValueCollection();
            ApplyTransformations(Nastaveni.SouborMetadat, headerFile, body, Nastaveni.DocasnaSlozka, parameters);*/
        }

        private static void UpravPerexSouboru(FileInfo souborXml, string outputFile)
        {
            var xws = new XmlWriterSettings();
            xws.NamespaceHandling = NamespaceHandling.OmitDuplicates;
            xws.CloseOutput = true;

            using (var xr = XmlReader.Create(souborXml.FullName))
            {
                using (var xr2 = XmlReader.Create(souborXml.FullName))
                {
                    using (var xw = XmlWriter.Create(outputFile, xws))
                    {
                        xw.WriteStartDocument();
                        while (xr.Read())
                        {
                            if (xr2.ReadState != ReadState.Closed)
                            {
                                xr2.Read();
                            }

                            if (xr.NodeType == XmlNodeType.Element && xr.Name == "sourceDesc" && xr.GetAttribute("n") == "characteristic")
                            {
                                ZpracujCharakteristikuNaPerex(xr2, xw);
                            }

                            Pomucky.Xml.Transformace.SerializeNode(xr, xw);
                        }

                        xw.WriteEndDocument();
                        xw.Close();
                    }
                }
            }
        }

        private static void ZpracujCharakteristikuNaPerex(XmlReader xr, XmlWriter xw)
        {
            xw.WriteStartElement("sourceDesc");
            xw.WriteAttributeString("n", "perex");

            var bylAnchor = false;
            var bylOdstavec = false;

            while (xr.Read())
            {
                var nazev = xr.Name;
                if (xr.NodeType == XmlNodeType.Element)
                    switch (nazev)
                    {
                        case "anchor":
                            if (xr.GetAttribute("type") == "predel")
                            {
                                Pomucky.Xml.Transformace.SerializeNode(xr, xw);
                                bylAnchor = true;
                            }
                            break;
                        default:
                            if (!bylAnchor)
                                Pomucky.Xml.Transformace.SerializeNode(xr, xw);
                            break;
                    }
                else
                {
                    if (xr.NodeType == XmlNodeType.EndElement)
                    {
                        if (nazev == "sourceDesc")
                        {
                            xw.WriteEndElement(); //</sourceDesc>
                            xr.Close();
                            return;
                        }
                        if (nazev == "p" && bylAnchor && !bylOdstavec)
                        {
                            Pomucky.Xml.Transformace.SerializeNode(xr, xw); //</p>
                            bylOdstavec = true;
                        }
                    }

                    if (!bylAnchor)
                    {
                        Pomucky.Xml.Transformace.SerializeNode(xr, xw);
                    }
                }
            }
        }

        private static void ZkontrolujCislaPagin(FileInfo souborXml, string outputFile)
        {
            Console.WriteLine("Upravuji čísla pagin v souboru {0}", souborXml.Name);

            var xws = new XmlWriterSettings();
            xws.NamespaceHandling = NamespaceHandling.OmitDuplicates;
            xws.CloseOutput = true;

            using (var xr = XmlReader.Create(souborXml.FullName))
            {
                using (var xw = XmlWriter.Create(outputFile, xws))
                {
                    xw.WriteStartDocument();
                    while (xr.Read())
                    {
                        Zacatek:
                        var nazev = xr.Name;
                        if (xr.NodeType == XmlNodeType.Element && nazev == "surface")
                        {
                            var n = xr.GetAttribute("n");
                            if (n == "xx" || n == "XX")
                            {
                                n = "××";
                            }
                            if (n.IndexOf('-') > -1)
                            {
                                n = n.Replace('-', '–');
                            }
                            if (n == "××") //obrázek označený ×× se vynechává
                            {
                                xr.Skip();
                                if (xr.NodeType == XmlNodeType.Element)
                                    goto Zacatek;
                                continue;
                            }
                            xw.WriteStartElement(nazev);
                            xw.WriteAttributeString("n", n);
                        }
                        else
                            Pomucky.Xml.Transformace.SerializeNode(xr, xw);
                    }
                }
            }
        }

        /// <summary>
        ///     Odliší hypertextové odkazy na stejně pojmenované paginy, popř. lišící se velikostí písmen
        /// </summary>
        private static void OdlisitStejnePojmenovanaPaginy(FileInfo souborXml, string outputFile)
        {
            Console.WriteLine("Odlišuju sejně pojmenované paginy v souboru {0}", souborXml.Name);

            var nmspc = new XmlNamespaceManager(new NameTable());
            nmspc.AddNamespace("t", "http://www.tei-c.org/ns/1.0");
            nmspc.AddNamespace("a", "http://vokabular.ujc.cas.cz/ns/anotace");
            nmspc.AddNamespace("xml", Objekty.XmlNamespace);

            var xd = new XmlDocument();
            xd.Load(souborXml.FullName);
            var odkazy = xd.SelectNodes("//t:surface/@n", nmspc);
            var pageNumbers = new Dictionary<string, List<XmlNode>>(odkazy.Count);

            char[] distinctChars = {'º', '¹', '²', '³', '⁴'};

            foreach (XmlNode odkaz in odkazy)
            {
                var text = odkaz.Value.ToUpper(CultureInfo.CurrentCulture);
                if (!pageNumbers.ContainsKey(text))
                {
                    pageNumbers.Add(text, new List<XmlNode>());
                }
                pageNumbers[text].Add(odkaz);
            }

            foreach (var pageNumber in pageNumbers)
            {
                if (pageNumber.Value.Count > 1)
                {
                    for (var i = 0; i < pageNumber.Value.Count; i++)
                    {
                        pageNumber.Value[i].Value += distinctChars[i];
                    }
                }
            }

            Console.WriteLine("\tPaginy přejmenovány");
            xd.Save(outputFile);
        }

        private static void UpravOdkazyNaPaginy(FileInfo souborXml, string outputFile)
        {
            Console.WriteLine("Upravuji odkazy v souboru {0}", souborXml.Name);

            var nmspc = GetMdmNamespaceManager();

            var xd = new XmlDocument();
            xd.Load(souborXml.FullName);
            var odkazy = xd.SelectNodes("//t:ref[@type='pagina']", nmspc);

            foreach (XmlNode node in odkazy)
            {
                var pagina = node.Attributes["target"].Value;
                if (PaginaExistuje(xd, nmspc, pagina) || PaginaExistuje(xd, nmspc, "–" + pagina) || PaginaExistuje(xd, nmspc, pagina + "–"))
                {
                    continue;
                }

                var xa = xd.CreateAttribute("subtype");
                xa.Value = "blind";
                node.Attributes.Append(xa);
            }

            xd.Save(outputFile);
        }

        private static bool PaginaExistuje(XmlDocument xd, XmlNamespaceManager nmspc, string cislo)
        {
            var xpath = string.Format("/t:TEI/t:facsimile/t:surface[@n='{0}']", cislo);
            if (cislo[cislo.Length - 1] == '–')
                xpath = string.Format("/t:TEI/t:facsimile/t:surface[starts-with(@n, '{0}')]", cislo);
            if (cislo[0] == '–')
            {
                xpath = string.Format("/t:TEI/t:facsimile/t:surface[substring(@n, string-length(@n) - string-length('{0}') + 1) = '{0}']", cislo); //= ends-with
            }
            var xnl = xd.SelectNodes(xpath, nmspc);
            if (xnl.Count == 1)
                return true;
            foreach (XmlNode node in xnl)
                if (node.Attributes["n"].Value.IndexOf('²') > -1)
                    return true;

            return false;
        }

        private static void AppendWatermarks(FileInfo souborxml, IEnumerable<string> images)
        {
            Console.WriteLine("Vytváření dávky pro vodoznak  {0}", souborxml.Name);

            var xd = new XmlDocument();
            var nmspc = GetMdmNamespaceManager();

            xd.Load(souborxml.FullName);
            var nd = xd.SelectSingleNode("/t:TEI/t:teiHeader/t:fileDesc/t:sourceDesc/t:msDesc/t:msIdentifier/t:repository", nmspc);
            var instituce = "SOUKROME";
            if (nd != null)
            {
                switch (nd.InnerText)
                {
                    case "Knihovna Ústavu pro jazyk český AV ČR, v. v. i.":
                    case "Ústav pro jazyk český AV ČR, v. v. i.":
                        instituce = "UJC";
                        break;
                    case "Knihovna Národního muzea":
                        instituce = "KNM";
                        break;
                    case "ze soukromé sbírky":
                        instituce = "SOUKROME";
                        break;
                    case "Státní oblastní archiv v Třeboni":
                        instituce = "TREBON";
                        break;
                    case "Vědecká knihovna v Olomouci":
                        instituce = "VKOL";
                        break;
                    case "Strahovská knihovna":
                        instituce = "STRAHOV";
                        break;
                    case "Moravská zemská knihovna":
                        instituce = "MZK";
                        break;
                    case "Bibliothek der Ludwig-Maximilians-Universität München":
                        instituce = "BLMU";
                        break;
                    case "Národní knihovna České republiky":
                        instituce = "NKCR";
                        break;
                    default:
                        if (!nd.InnerText.StartsWith("ze soukromé sbírky"))
                        {
                            throw new Exception(string.Format("Neznámá instituce '{0}' v souboru {1}", nd.InnerText, souborxml.Name));
                        }

                        break;
                }
            }

            AddWatermark(images, string.Format("Vodoznak_{0}.png", instituce));
        }

        private static XmlNamespaceManager GetMdmNamespaceManager()
        {
            var nmspc = new XmlNamespaceManager(new NameTable());
            nmspc.AddNamespace("t", "http://www.tei-c.org/ns/1.0");
            nmspc.AddNamespace("a", "http://vokabular.ujc.cas.cz/ns/anotace");
            nmspc.AddNamespace("xml", Objekty.XmlNamespace);

            return nmspc;
        }

        private static void AddWatermark(IEnumerable<string> images, string instituce)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var watermarkStreamResource = assembly.GetManifestResourceStream("Daliboris.Texty.Export.Watermarks." + instituce);

            if (watermarkStreamResource == null)
            {
                throw new Exception(string.Format("Watermark Daliboris.Texty.Export.Watermarks.{0} not found", instituce));
            }
            var imageWatermark = Image.FromStream(watermarkStreamResource);
            var watermark = new ImageWatermark(imageWatermark);
            watermark.ContentAlignment = ContentAlignment.BottomCenter;
            watermark.Alpha = 99F;

            foreach (var image in images)
            {
                var fi = new FileInfo(image);

                var tempFile = fi.FullName.Substring(0, fi.FullName.Length - fi.Extension.Length) + ".help" + fi.Extension;

                File.Move(fi.FullName, tempFile);

                watermark.SaveProcessedImageToFileSystem(tempFile, fi.FullName);

                File.Delete(tempFile);
            }
        }
    }
}