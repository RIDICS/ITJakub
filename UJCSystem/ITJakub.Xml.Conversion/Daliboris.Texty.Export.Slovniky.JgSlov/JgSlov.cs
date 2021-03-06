﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Daliboris.Pomucky.Xml;
using Daliboris.Texty.Export;
using Daliboris.Texty.Export.Rozhrani;
using Ujc.Ovj.ChangeEngine.Objects;
using Ujc.Ovj.Tools.Xml.XsltTransformation;

namespace Daliboris.Slovniky
{
    public class JgSlov : MockDictionary
    {
        protected string m_changeRuleSetFile = "Daliboris.Slovniky.Xmr.JgSlov_pravidla_v4.xmr";
        private string m_TEI_ClassificationDeclarations = "Daliboris.Slovniky.Xls.TEI_ClassificationDeclarations.xsl";

        public JgSlov()
        {
            UsePersonalizedXmdGenerator = true;
        }

        public override void SeskupitHeslaPismene(string inputFile, string outputFile, string filenameWithoutExtension)
        {
            var fiOutputFile = new FileInfo(outputFile);
            var outputFileName = fiOutputFile.Name.Substring(0, fiOutputFile.Name.Length - fiOutputFile.Extension.Length);
            var keywordFile = Path.Combine(fiOutputFile.DirectoryName, string.Format("{0}_Keyword{1}", outputFileName, fiOutputFile.Extension));
            
            TestExtrahujHesla(inputFile, keywordFile, filenameWithoutExtension, m_changeRuleSetFile);

            var termFile = Path.Combine(fiOutputFile.DirectoryName, string.Format("{0}_Term{1}", outputFileName, fiOutputFile.Extension));
            TestExtrahujTerminy(keywordFile, termFile);
            
            TestSloucitFacsimileATerminy(string.Format("Daliboris.Slovniky.Xml.{0}.xml", filenameWithoutExtension), keywordFile, outputFile);
        }

        /// <summary>
        ///     Extrahuje hesla a podheslí z
        /// </summary>
        /// <param name="identifikatorDilu"></param>
        /// <param name="changeRuleSetFile"></param>
        public void TestExtrahujHesla(string inputFile, string outputFile, string identifikatorDilu, string changeRuleSetFile, bool generateHeader=true)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var changeRuleSet = ChangeRuleSet.Load(assembly.GetManifestResourceStream(changeRuleSetFile));

            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var identifikatory = new Dictionary<string, Guid>(10000);
            List<string> heslaPaginy = null;

            //string sSoubor = @"D:\Slovniky\JgSlov\Data\JgSlov_Transkripce.xml";
            //Transformator trs = NactiNovaPravidla(sSoubor);
            
            using (var xw = XmlWriter.Create(outputFile, xws))
            {
                xw.WriteStartDocument();

                if (generateHeader)
                {
                    xw.WriteStartElement("TEI", "http://www.tei-c.org/ns/1.0");

                    //xw.WriteAttributeString("xmlns", "http://www.tei-c.org/ns/1.0");
                    var xd = new XmlDocument();
                    xd.LoadXml(DejHlavicku(identifikatorDilu));
                    xd.WriteContentTo(xw);
                }

                xw.WriteStartElement("facsimile", "http://www.tei-c.org/ns/1.0");
                xw.WriteAttributeString("n", identifikatorDilu);

                using (var xr = XmlReader.Create(inputFile))
                {
                    var pagina = "0";
                    var pismeno = "M";

                    var divId = "body";
                    var divLevel = 0;

                    string lastEntryId = "";
                    XmlReader lastForm = null;

                    var iHeslo = 1;
                    var surfaceIsOpen = false;
                    var descIsOpen = false;
                    xr.MoveToContent();

                    while (xr.Read())
                    {
                        var nodeName = xr.Name;
                        if (xr.NodeType == XmlNodeType.Element)
                        {
                            switch (nodeName)
                            {
                                case "div":
                                    if (!xr.IsEmptyElement)
                                    {
                                        divLevel++;
                                    }
                                    divId = xr.GetAttribute("xml:id");

                                    break;
                                case "head":
                                case "Pismeno":
                                    if (nodeName == "head" && divLevel != 2)
                                    {
                                        break;
                                    }

                                    pismeno = Objekty.ReadCurrentNodeContentAsString(xr).ToUpper();
                                    if (pismeno.IndexOf('(') > -1)
                                    {
                                        pismeno = pismeno.Substring(0, pismeno.IndexOf('(') - 1).Trim();
                                    }
                                    if (pismeno.EndsWith("."))
                                        pismeno = pismeno.Substring(0, pismeno.Length - 1);

                                    break;
                                case "pb":
                                case "Paginace":
                                    heslaPaginy = new List<string>(120);
                                    if (descIsOpen)
                                    {
                                        xw.WriteEndElement(); //desc
                                        descIsOpen = false;
                                    }
                                    if (surfaceIsOpen)
                                    {
                                        xw.WriteStartElement("graphic");

                                        if (identifikatorDilu != "DDBW") //DDBW has not have graphic
                                        {
                                            xw.WriteAttributeString("url", DejNazevSouboru(pagina, identifikatorDilu));
                                        }

                                        xw.WriteEndElement(); //graphic

                                        xw.WriteEndElement(); //surface
                                        surfaceIsOpen = false;
                                    }

                                    pagina = nodeName == "pb"
                                        ? xr.GetAttribute("n")
                                        : Objekty.ReadCurrentNodeContentAsString(xr).Trim();
                                    xw.WriteStartElement("surface");
                                    surfaceIsOpen = true;
                                    xw.WriteAttributeString("n", pagina);
                                    xw.WriteStartElement("desc");
                                    descIsOpen = true;

                                    break;
                                case "entryFree":
                                    lastEntryId= xr.GetAttribute("xml:id");

                                    break;
                                case "form":
                                    lastForm = xr.ReadSubtree();

                                    break;
                                case "orth":
                                case "heslo":
                                case "podhesli":
                                    var heslo = Objekty.ReadCurrentNodeContentAsString(xr).Trim();
                                    heslo = UppercaseFirst(heslo);
                                    var type = "main";

                                    if (nodeName == "podhesli")
                                        type = "detail";

                                    var pocatecniPismeno = RemoveNonLetters(heslo);

                                    if (pocatecniPismeno.Length == 0) break; //chyba - to by se nemělo stát; TODO

                                    pocatecniPismeno = DejPocatecniPismeno(pocatecniPismeno, true);

                                    // string identifikator = String.Format("{0}|{1}|{2}", heslo.ToLower(), type, pismeno);
                                    var identifikator = string.Format("{0}|{1}|{2}", heslo.ToLower(), type, pocatecniPismeno);

                                    if (!identifikatory.ContainsKey(identifikator))
                                        identifikatory.Add(identifikator, Guid.NewGuid());

                                    //na stránce se v MDM nesmějí vyskytovat duplicitní heslo
                                    //nebo to udělat tak, že (pod)heslo vždycky dostane jedinečné GUID
                                    if (!heslaPaginy.Contains(identifikator))
                                    {
                                        xw.WriteStartElement("term");
                                        xw.WriteAttributeString("id", identifikatory[identifikator].ToString("D"));
                                        xw.WriteAttributeString("type", type);

                                        //xw.WriteAttributeString("subtype", pismeno);
                                        xw.WriteAttributeString("subtype", pocatecniPismeno);

                                        string international = null;

                                        //international = trs.AplikujPravidla(heslo, "cze");

                                        if (nodeName == "orth") //die if not exist parent <form> for <orth>
                                        {
                                            xw.WriteAttributeString("n", lastEntryId);
                                            
                                            while (lastForm.Read())
                                            {
                                                if (lastForm.NodeType == XmlNodeType.Element)
                                                {
                                                    if (lastForm.Name == "reg")
                                                    {
                                                        international = Objekty.ReadCurrentNodeContentAsString(lastForm).Trim();
                                                    }
                                                }
                                            }
                                        }
                                        international = international ?? changeRuleSet.Apply(heslo);

                                        xw.WriteAttributeString("international", international);

                                        if (nodeName != "orth")
                                        {
                                            xw.WriteAttributeString("n", string.Format("{0:000000}", iHeslo++));
                                        }
                                        xw.WriteString(heslo);
                                        xw.WriteEndElement(); //term
                                        heslaPaginy.Add(identifikator);
                                    }

                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (xr.NodeType == XmlNodeType.EndElement)
                        {
                            switch (nodeName)
                            {
                                case "div":
                                    divLevel--;

                                    break;
                            }
                        }
                    }
                }
                xw.WriteEndElement(); //facsimile
                if (generateHeader)
                {
                    xw.WriteEndElement(); //TEI
                }
                xw.WriteEndDocument();
            }
        }

        private string UppercaseFirst(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return string.Empty;
            }

            var a = word.ToLower(CultureInfo.CurrentCulture).ToCharArray();
            for (var i = 0; i < a.Length; i++)
            {
                if (char.IsLetter(word[i]))
                {
                    if (a[0] != '-')
                        a[i] = char.ToUpper(a[i]);
                    break;
                }
            }

            return new string(a);
        }

        public static void HeslovaSlovaProTranskripci(string vstup, string vystup)
        {
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            char[] znaky = {'\x1d'};

            using (var xw = XmlWriter.Create(vystup, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("changedTokens");

                using (var sr = new StreamReader(vstup))
                {
                    string radek = null;
                    while ((radek = sr.ReadLine()) != null)
                    {
                        var polozky = radek.Split(znaky);
                        xw.WriteStartElement("changedToken");
                        xw.WriteStartElement("source");
                        xw.WriteAttributeString("text", polozky[0]);
                        xw.WriteAttributeString("frequency", polozky[1]);
                        xw.WriteAttributeString("lang", "cs");
                        xw.WriteEndElement(); //source
                        xw.WriteEndElement(); //changedToken
                    }
                }

                xw.WriteEndElement(); //ArrayOfChangedToken
                xw.WriteEndDocument();
            }
        }

        protected virtual string DejHlavicku(string identifikatorDilu)
        {
            const string hlavicka = @"
<teiHeader xmlns='http://www.tei-c.org/ns/1.0' xml:id='{3}' n='{5}'>
<fileDesc n='{2}'>
			<titleStmt>
                <title type='translit'>Slownjk česko-německý, {6}</title>
				<title>Slovník česko-německý, {0}</title>
				<author><forename>Josef</forename><surname>Jungmann</surname></author>
			</titleStmt>
			<editionStmt>
				<edition>digitální mluvnice</edition>
			</editionStmt>
			<publicationStmt>
				<publisher>oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.<email>vyvoj@ujc.cas.cz</email></publisher>
				<pubPlace>Praha</pubPlace>
				<date>2011</date>
				<availability status='restricted'>
					<p>Tato edice je autorské dílo chráněné ve smyslu zákona č. 121/2000 Sb., o právu autorském, a je určena pouze k nekomerčním účelům.</p>
				</availability>
			</publicationStmt>
			<sourceDesc n='bibl'>
				<p><bibl type='source'><author><forename>Josef</forename>, <surname>Jungmann</surname></author>. <title>Slovník česko-německý, {0}</title> <pubPlace>Praha</pubPlace>, <publisher>Knížecí arcibiskupská knihtiskárna</publisher> <date>{1}</date>. <edition>Vydání první.</edition></bibl></p>
			</sourceDesc>
			<sourceDesc>
				<msDesc xml:lang='cs'>
					<msIdentifier>
						<country key='xr'>Česko</country>
						<settlement>Praha</settlement>
						<repository>Ústav pro jazyk český AV ČR, v. v. i.</repository>
						<idno>{4}</idno>
					</msIdentifier>
					<msContents>
						<msItem>
							<title>Slovník česko-německý, {0}</title>
							<author><forename>Josef</forename><surname>Jungmann</surname></author>
						</msItem>
					</msContents>
					<history>
						<origin><origDate notBefore='{1}' notAfter='{1}'>{1}</origDate></origin>
					</history>
                    <additional>
                         <adminInfo>
                             <recordHist>
                                <p><persName>Boris Lehečka</persName>, <date>{7}</date><note>základní převod na XML</note></p>
                            </recordHist>
                        </adminInfo>
                    </additional>
				</msDesc>
			</sourceDesc>
		</fileDesc>
        <encodingDesc>
        {8}
        </encodingDesc>
		<profileDesc>
            <textClass>
                <catRef target='#taxonomy-dictionary-contemporary #output-dictionary' />
            </textClass>
            <langUsage>
				<language ident='cs' usage='80'>čeština</language>
				<language ident='de' usage='20'>němčina</language>
                <language ident='la' usage='10'>latina</language>
			</langUsage>
		</profileDesc>
	</teiHeader>
";
            var guid = "{7AC74E21-53E9-4E4D-A7CC-CA739A962E58}";
            var dilTranslit = "Djl I., A–J";
            var dilTranskr = "Díl I., A–J";
            var vroceni = "1835";
            var identifikatorTeiHeader = "JgSlov01";
            var signatura = "TS III 65/1";
            var zkratka = "JgSlov 1";
            var aktualizace = DateTime.Today.ToShortDateString();

            if (identifikatorDilu == "JgSlov02")
            {
                guid = "{44BA5794-DF0F-4A30-A1C6-47CE2E83055C}";
                dilTranslit = "Djl II., K–O";
                dilTranskr = "Díl II., K–O";
                vroceni = "1836";
                identifikatorTeiHeader = "JgSlov02";
                signatura = "TS III 65/2";
                zkratka = "JgSlov 2";
            }

            if (identifikatorDilu == "JgSlov03")
            {
                guid = "{DF2376C7-46E8-48A6-A63C-EF9ABF8C71CA}";
                dilTranslit = "Djl III., P–R";
                dilTranskr = "Díl III., P–R";
                vroceni = "1837";
                identifikatorTeiHeader = "JgSlov03";
                signatura = "TS III 65/3";
                zkratka = "JgSlov 3";
            }

            if (identifikatorDilu == "JgSlov04")
            {
                guid = "{F01FA035-099D-4A53-A48A-878D76A1324A}";
                dilTranslit = "Djl IV., S–U";
                dilTranskr = "Díl IV., S–U";
                vroceni = "1838";
                identifikatorTeiHeader = "JgSlov04";
                signatura = "TS III 65/4";
                zkratka = "JgSlov 4";
            }

            if (identifikatorDilu == "JgSlov05")
            {
                guid = "{DB921B03-F9F3-4898-9C94-225DCC2AD5E7}";
                dilTranslit = "Djl V., W–Z";
                dilTranskr = "Díl V., V–Z";
                vroceni = "1839";
                identifikatorTeiHeader = "JgSlov05";
                signatura = "TS III 65/5";
                zkratka = "JgSlov 5";
            }
            
            return string.Format(hlavicka, dilTranskr, vroceni, guid, identifikatorTeiHeader, signatura, zkratka, dilTranslit, aktualizace, GetTEIClassificationDeclarations());
        }

        protected string GetTEIClassificationDeclarations()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(assembly.GetManifestResourceStream(m_TEI_ClassificationDeclarations));

            return xmlDocument.GetElementsByTagName("classDecl")[0].OuterXml;
        }

        /// <summary>
        ///     Termíny = vstupní soubor, facsimile = výstupní soubor.
        /// </summary>
        /// <param name="vystup"></param>
        protected virtual void TestSloucitFacsimileATerminy(string facsimileXml, string terminyXml, string vystup)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            var facsimile = new XmlDocument();
            var terminy = new XmlDocument();

            terminy.Load(terminyXml);
            facsimile.Load(assembly.GetManifestResourceStream(facsimileXml));

            var namespaceManager = new XmlNamespaceManager(terminy.NameTable);
            namespaceManager.AddNamespace("t", "http://www.tei-c.org/ns/1.0");

            var remove = facsimile.SelectNodes("//t:surface[starts-with(@n, '×')]", namespaceManager);

            for (var i = remove.Count - 1; i >= 0; i--)
            {
                var parentNode = remove[i].ParentNode;
                if (parentNode != null) parentNode.RemoveChild(remove[i]);
            }

            var surfaces = facsimile.SelectNodes("//t:surface", namespaceManager);

            XmlNode importNode;
            foreach (XmlNode surface in surfaces)
            {
                var xpath = string.Format("//t:surface[@n='{0}']/t:desc", surface.Attributes["n"].Value);
                var findSurface = terminy.SelectSingleNode(xpath, namespaceManager);
                if (findSurface != null)
                {
                    importNode = surface.OwnerDocument.ImportNode(findSurface, true);
                    var surfaceDesc = surface.SelectSingleNode("./t:desc", namespaceManager);
                    var desc = surface.ReplaceChild(importNode, surfaceDesc);
                }
            }

            var terminyFacsimile = terminy.SelectSingleNode("/t:TEI/t:facsimile", namespaceManager);
            importNode = terminy.ImportNode(facsimile.DocumentElement, true);
            var terminyTEI = terminy.SelectSingleNode("/t:TEI", namespaceManager);
            terminyFacsimile.ParentNode.ReplaceChild(importNode, terminyFacsimile);

            var obrazky = terminy.SelectNodes("//t:graphic", namespaceManager);
            foreach (XmlNode obrazek in obrazky)
            {
                var url = obrazek.Attributes["url"].Value;
                obrazek.Attributes["url"].Value = url.Substring(url.LastIndexOf('\\') + 1);
            }
            terminy.Save(vystup);
        }

        /// <summary>
        ///     Uloží termíny do samostatného souboru, odstraní prázdné xmlns, pouze s jedinečnými identifikátory
        /// </summary>
        public void TestExtrahujTerminy(string inputFile, string outputFile)
        {
            var xws = new XmlWriterSettings();
            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("", "http://vokabular.ujc.cas.cz/ns/anotace");

            xws.Indent = true;
            var identifikatory = new Dictionary<string, string>(10000);
            List<string> heslaPaginy = null;
            using (var xw = XmlWriter.Create(outputFile, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("terms");

                using (var xr = XmlReader.Create(inputFile))
                {
                    xr.MoveToContent();
                    Dalsi:
                    while (xr.Read())
                    {
                        if (xr.NodeType == XmlNodeType.Element && xr.Name == "term")
                        {
                            var guid = xr.GetAttribute("id");
                            if (!identifikatory.ContainsKey(guid))
                            {
                                identifikatory.Add(guid, guid);
                                xr.MoveToElement();
                                var xd = Objekty.ReadNodeAsXmlDocument(xr);
                                xd.DocumentElement.RemoveAttribute("xmlns", "");
                                xw.WriteNode(xd.DocumentElement.CreateNavigator(), false);
                                goto Dalsi;
                                //xw.WriteNode(xr, false);
                            }
                        }
                    }
                }
                xw.WriteEndElement(); //terms
                xw.WriteEndDocument();
            }
        }

        private static string DejNazevSouboru(string pagina, string identifikatorDilu)
        {
            //string slozka = @"X:\Temp\JgSlov\Slovniky\JgSlov\Skeny\Jungmann_02\";
            var pocatecniStrana = 18;
            if (identifikatorDilu == "Jungmann_02")
                pocatecniStrana = 11;
            int pageNumber;
            if (!int.TryParse(pagina, out pageNumber))
            {
                return string.Empty;
            }
            var soucin = pageNumber*2 + pocatecniStrana;
            if (pageNumber%2 == 1)
                soucin = soucin + 1;
            var cislo = string.Format("{0:00}_{1:00000}.jpg", identifikatorDilu.Replace("Jungmann_", "JgSlov"), soucin);
            //return slozka + cislo;
            return cislo;
        }

        /*
        private static Transformator NactiNovaPravidla(string sSoubor)
        {
            Transformator trs = new Transformator();
            Pravidla prs = new Pravidla();
            prs = Spravce.Deserializuj(typeof(Pravidla), sSoubor) as Pravidla;
            trs.Pravidla = prs;

            return trs;
        }
        */

        protected string GetTempFile(string tempDirectory, string sourceFile, int step)
        {
            const string fileNameFormat = "{0}_{1:00}.xmd";

            return Path.Combine(tempDirectory, string.Format(fileNameFormat, sourceFile, step));
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
            var parameters = new NameValueCollection
            {
                {"accessories", finalOutputFileName}
            };

            foreach (var transformationFile in XsltTransformerFactory.GetTransformationFromTransformationsFile(settings.SouborTransformaci, "jgslov-xmd-step"))
            {
                var fileTransformationTarget = GetTempFile(settings.DocasnaSlozka, outputFileWithoutExtension, step++);

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