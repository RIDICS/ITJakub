using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Daliboris.Transkripce;
using Daliboris.Transkripce.Objekty;

namespace Daliboris.Slovniky
{
    public class JgSlov : Slovnik
    {

        public JgSlov() { }

        public JgSlov(string vstupniSoubor)
        {
            base.VstupniSoubor = vstupniSoubor;
        }
        public JgSlov(string vstupniSoubor, string vystupniSoubor)
        {
            base.VstupniSoubor = vstupniSoubor;
            base.VystupniSoubor = vystupniSoubor;
        }

	    /// <summary>
	    /// Extrahuje hesla a podheslí z 
	    /// </summary>
	    /// <param name="identifikatorDilu"> </param>
	    /// <param name="changeRuleSetFile"></param>
	    public void TestExtrahujHesla(string identifikatorDilu, string changeRuleSetFile)
        {
            /*
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            Dictionary<string, Guid> identifikatory = new Dictionary<string, Guid>(10000);
            List<string> heslaPaginy = null;

            //string sSoubor = @"D:\Slovniky\JgSlov\Data\JgSlov_Transkripce.xml";
            //Transformator trs = NactiNovaPravidla(sSoubor);

		    ChangeRuleSet changeRuleSet = ChangeRuleSet.Load(changeRuleSetFile);

            using (XmlWriter xw = XmlWriter.Create(VystupniSoubor, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("TEI", "http://www.tei-c.org/ns/1.0");

                //xw.WriteAttributeString("xmlns", "http://www.tei-c.org/ns/1.0");
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(DejHlavicku(identifikatorDilu));
                xd.WriteContentTo(xw);

                xw.WriteStartElement("facsimile");
                xw.WriteAttributeString("n", identifikatorDilu);

                using (XmlReader xr = XmlReader.Create(VstupniSoubor))
                {
                    string pagina = "0";
                    string pismeno = "M";
                    int iHeslo = 1;
                    bool surfaceIsOpen = false;
                    bool descIsOpen = false;
                    xr.MoveToContent();
                    while (xr.Read())
                    {

                        string nodeName = xr.Name;
                        if (xr.NodeType == XmlNodeType.Element)
                        {
                            switch (nodeName)
                            {
                                case "Pismeno":
                                    pismeno = Pomucky.Xml.Objekty.ReadCurrentNodeContentAsString(xr).ToUpper();
                                    if (pismeno.IndexOf('(') > -1)
                                    {
                                        pismeno = pismeno.Substring(0, pismeno.IndexOf('(') - 1).Trim();
                                    }
                                    if (pismeno.EndsWith("."))
                                        pismeno = pismeno.Substring(0, pismeno.Length - 1);
                                    break;
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
                                        xw.WriteAttributeString("url", DejNazevSouboru(pagina, identifikatorDilu));
                                        xw.WriteEndElement(); //graphic

                                        xw.WriteEndElement(); //surface
                                        surfaceIsOpen = false;
                                    }

                                    pagina = Pomucky.Xml.Objekty.ReadCurrentNodeContentAsString(xr).Trim();
                                    xw.WriteStartElement("surface");
                                    surfaceIsOpen = true;
                                    xw.WriteAttributeString("n", pagina);
                                    xw.WriteStartElement("desc");
                                    descIsOpen = true;
                                    break;
                                case "heslo":
                                case "podhesli":
                                    string heslo = Pomucky.Xml.Objekty.ReadCurrentNodeContentAsString(xr).Trim();
                                    heslo = UppercaseFirst(heslo);
                                    string type = "main";
                                    if (nodeName != "heslo")
                                        type = "detail";
                                    string pocatecniPismeno = RemoveNonLetters(heslo);

                                    if (pocatecniPismeno.Length == 0) break; //chyba - to by se nemělo stát; TODO

                                    pocatecniPismeno = DejPocatecniPismeno(pocatecniPismeno, true);

                                    // string identifikator = String.Format("{0}|{1}|{2}", heslo.ToLower(), type, pismeno);
                                    string identifikator = String.Format("{0}|{1}|{2}", heslo.ToLower(), type, pocatecniPismeno);

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
                                        
                                        string international;
                                        
                                        //international = trs.AplikujPravidla(heslo, "cze");

                                        international = changeRuleSet.Apply(heslo);

                                        xw.WriteAttributeString("international", international);

                                        xw.WriteAttributeString("n", String.Format("{0:000000}", iHeslo++));
                                        xw.WriteString(heslo);
                                        xw.WriteEndElement(); //term
                                        heslaPaginy.Add(identifikator);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                xw.WriteEndElement(); //facsimile
                xw.WriteEndElement(); //TEI
                xw.WriteEndDocument();
            }
            */
        }

        private string UppercaseFirst(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return string.Empty;
            }

            char[] a = word.ToLower(CultureInfo.CurrentCulture).ToCharArray();
            for (int i = 0; i < a.Length; i++)
            {
                if (Char.IsLetter(word[i]))
                {
					if(a[0] != '-')
						a[i] = char.ToUpper(a[i]);
                    break;
                }
            }
            
            return new string(a);
        }

        public static void HeslovaSlovaProTranskripci(string vstup, string vystup)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            char[] znaky = new char[] { '\x1d' };

            using (XmlWriter xw = XmlWriter.Create(vystup, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("changedTokens");

                using (StreamReader sr = new StreamReader(vstup))
                {
                    string radek = null;
                    while ((radek = sr.ReadLine()) != null)
                    {
                        string[] polozky = radek.Split(znaky);
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

        private string DejHlavicku(string identifikatorDilu)
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
		<profileDesc>
			<langUsage>
				<language ident='cs' usage='80'>čeština</language>
				<language ident='de' usage='20'>němčina</language>
                <language ident='la' usage='10'>latina</language>
			</langUsage>
		</profileDesc>
	</teiHeader>
";
            string guid = "{7AC74E21-53E9-4E4D-A7CC-CA739A962E58}";
            string dilTranslit = "Djl I., A–J";
            string dilTranskr = "Díl I., A–J";
            string vroceni = "1835";
            string identifikatorTeiHeader = "JgSlov01";
            string signatura = "TS III 65/1";
            string zkratka = "JgSlov 1";
            string aktualizace = DateTime.Today.ToShortDateString();

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

            return string.Format(hlavicka, dilTranskr, vroceni, guid, identifikatorTeiHeader, signatura, zkratka, dilTranslit, aktualizace);
        }

        /// <summary>
        /// Termíny = vstupní soubor, facsimile = výstupní soubor.
        /// </summary>
        /// <param name="vystup"></param>
        public void TestSloucitFacsimileATerminy(string facsimileXml, string terminyXml, string vystup)
        {
            XmlDocument facsimile = new XmlDocument();
            XmlDocument terminy = new XmlDocument();

            terminy.Load(terminyXml);
            facsimile.Load(facsimileXml);

            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(terminy.NameTable);
            namespaceManager.AddNamespace("t", "http://www.tei-c.org/ns/1.0");

            XmlNodeList remove = facsimile.SelectNodes("//t:surface[starts-with(@n, '×')]", namespaceManager);

            for (int i = remove.Count - 1; i >= 0; i--)
            {
                var parentNode = remove[i].ParentNode;
                if (parentNode != null) parentNode.RemoveChild(remove[i]);
            }

            XmlNodeList surfaces = facsimile.SelectNodes("//t:surface", namespaceManager);

            XmlNode importNode;
            foreach (XmlNode surface in surfaces)
            {
                string xpath = String.Format("//t:surface[@n='{0}']/t:desc", surface.Attributes["n"].Value);
                XmlNode findSurface = terminy.SelectSingleNode(xpath, namespaceManager);
                if (findSurface != null)
                {
                    importNode = surface.OwnerDocument.ImportNode(findSurface, true);
                    XmlNode surfaceDesc = surface.SelectSingleNode("./t:desc", namespaceManager);
                    XmlNode desc = surface.ReplaceChild(importNode, surfaceDesc);
                }
            }

            XmlNode terminyFacsimile = terminy.SelectSingleNode("/t:TEI/t:facsimile", namespaceManager);
            importNode = terminy.ImportNode(facsimile.DocumentElement, true);
            XmlNode terminyTEI = terminy.SelectSingleNode("/t:TEI", namespaceManager);
            terminyFacsimile.ParentNode.ReplaceChild(importNode, terminyFacsimile);

            XmlNodeList obrazky = terminy.SelectNodes("//t:graphic", namespaceManager);
            foreach (XmlNode obrazek in obrazky)
            {
                string url = obrazek.Attributes["url"].Value;
                obrazek.Attributes["url"].Value = url.Substring(url.LastIndexOf('\\') + 1);
            }
            terminy.Save(vystup);

        }

        /// <summary>
        /// Uloží termíny do samostatného souboru, odstraní prázdné xmlns, pouze s jedinečnými identifikátory
        /// </summary>
        public void TestExtrahujTerminy()
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("", "http://vokabular.ujc.cas.cz/ns/anotace");

            xws.Indent = true;
            Dictionary<string, string> identifikatory = new Dictionary<string, string>(10000);
            List<string> heslaPaginy = null;
            using (XmlWriter xw = XmlWriter.Create(VystupniSoubor, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("terms");

                using (XmlReader xr = XmlReader.Create(VstupniSoubor))
                {
                    xr.MoveToContent();
                Dalsi:
                    while (xr.Read())
                    {
                        if (xr.NodeType == XmlNodeType.Element && xr.Name == "term")
                        {
                            string guid = xr.GetAttribute("id");
                            if (!identifikatory.ContainsKey(guid))
                            {
                                identifikatory.Add(guid, guid);
                                xr.MoveToElement();
                                XmlDocument xd = Pomucky.Xml.Objekty.ReadNodeAsXmlDocument(xr);
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
            int pocatecniStrana = 18;
            if (identifikatorDilu == "Jungmann_02")
                pocatecniStrana = 11;
            int pageNumber;
            if (!Int32.TryParse(pagina, out  pageNumber))
            {
                return String.Empty;
            }
            int soucin = (pageNumber * 2) + pocatecniStrana;
            if (pageNumber % 2 == 1)
                soucin = soucin + 1;
            string cislo = String.Format("{0:00}_{1:00000}.jpg", identifikatorDilu.Replace("Jungmann_", "JgSlov"), soucin);
            //return slozka + cislo;
            return cislo;
        }

        public override void UpravitHraniceHesloveStati(string inputFile, string outputFile)
        {
            throw new NotImplementedException();
        }

        public override void KonsolidovatHeslovouStat(string inputFile, string outputFile)
        {
            throw new NotImplementedException();
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
    }
}
