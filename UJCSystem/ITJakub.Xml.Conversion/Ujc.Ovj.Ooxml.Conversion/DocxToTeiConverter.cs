using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Daliboris.OOXML.Word.Transform;
using Daliboris.Texty.Evidence;
using Daliboris.Texty.Evidence.Rozhrani;
using Daliboris.Texty.Export;
using Daliboris.Texty.Export.Rozhrani;
using Daliboris.Texty.Export.SlovnikovyModul;
using Daliboris.Transkripce.Objekty;
using JetBrains.Annotations;
using Ujc.Ovj.Xml.Tei.Contents;
using Ujc.Ovj.Xml.Tei.Splitting;
using Prepis = Daliboris.Texty.Evidence.Prepis;
using Prepisy = Daliboris.Texty.Evidence.Prepisy;

namespace Ujc.Ovj.Ooxml.Conversion
{
	public class VersionInfoSkeleton
	{

		const string XmlExtenstion = ".xml";

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public VersionInfoSkeleton()
		{
			Id = GenerateId(DateTime.UtcNow);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public VersionInfoSkeleton(string message, DateTime creation)
			: this()
		{
			Message = message;
			Creation = creation;
			Id = GenerateId(creation);
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public VersionInfoSkeleton(string message, DateTime creation, string id)
			: this(message, creation)
		{
			Id = id;
		}

		public string Message { get; set; }
		public string Id { get; set; }
		public DateTime Creation { get; set; }

		private static string GenerateId(DateTime dateTime)
		{
			return String.Format("Change_{0:O}", dateTime);
		}

	}

	public class DocxToTeiConverter
	{

		XNamespace nsTei = "http://www.tei-c.org/ns/1.0";
		XNamespace nsXml = "http://www.w3.org/XML/1998/namespace";
		XNamespace nsItj = "http://vokabular.ujc.cas.cz/ns/it-jakub/1.0";
		XNamespace nsNlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";


		private ConversionResult _result;
		private const string XmlExtension = ".xml";
		private string _documentId = null;
		private VersionInfoSkeleton _currentVersionInfoSkeleton;

		public string GetDocumentId()
		{
			return _documentId;
		}

		public ConversionResult Convert(DocxToTeiConverterSettings settings)
		{

			_result = new ConversionResult();
			string documentType = null;

			FileInfo inputFileInfo = new FileInfo(settings.InputFilePath);

			string inputFileName = inputFileInfo.Name;

			ResolveDefaultSettingsValues(settings);

			List<IPrepis> glsPrepisy = GetPrepisy(settings, inputFileName);

			if (glsPrepisy == null || glsPrepisy.Count == 0 || glsPrepisy[0] == null)
			{
				//dokument v evidenci neexistuje, nabídnout zanesení dokumentu do evidence
				// mělo by stačit přiřazení typu dokumentu
				_result.Errors.Add(new DocumentNotInEvidenceException("Dokument s uvedeným jménem souboru neexistuje v evidenci."));
				return _result;
			}

			IPrepis prepis = glsPrepisy[0];

			documentType = GetDocumentType(prepis.TypPrepisu);
			_documentId = prepis.GUID;

			if (documentType == null)
			{
				//dokument má v evidenci přiřazen typ dokumentu, který není podporován
				_result.Errors.Add(new NotSupportedFileFormatException("Dokument má v evidenci přiřazen typ dokumentu, který není podporován."));
				return _result;
			}

			string tempDirectoryPath = settings.TempDirectoryPath;
			//vytvoří se adresářová struktura, pokud neexistuje, pro ukládání výsledných a dočasných souborů
			AdresarovaStruktura ads = new AdresarovaStruktura(tempDirectoryPath, documentType);
			ads.VytvorStrukturu();


			string docxToXmlFilePath = Path.Combine(GetDataDirectoryPath(), "AllStylesConvert.2xml");
			string xsltTemplatesPath = GetXsltTemplatesPath();
			string xsltTransformationsPath = GetXsltTransformationsPath();



			string fileNameWithoutExtension = prepis.Soubor.NazevBezPripony;
			string xmlOutpuFileName = fileNameWithoutExtension + XmlExtension;

			string docxToXmlOutput = Path.Combine(ads.DejSpolecneDocXml, xmlOutpuFileName);
			string finalOutputDirectory = ads.DejVystup; // Path.Combine(ads.DejVystup, fileNameWithoutExtension);
			string finalOutputFileName = Path.Combine(finalOutputDirectory, xmlOutpuFileName);

			//Zatím pouze konverze z DOCX do základního XML
			try
			{
				ConvertDocxToXml(settings.InputFilePath, docxToXmlFilePath, docxToXmlOutput);
			}
			catch (Exception exception)
			{
				_result.Errors.Add(exception);
				return _result;
			}

			if (!Directory.Exists(finalOutputDirectory))
				Directory.CreateDirectory(finalOutputDirectory);

			IExportNastaveni exportSettings = GetExportSettings(documentType, settings, xsltTransformationsPath, xsltTemplatesPath, ads, glsPrepisy);
			ExportBase export = GetExportModule(documentType, exportSettings);

			if (export == null || exportSettings == null)
			{
				//Objekt pro export se nepodažřilo vytvořit, není podporován.
				return _result;
			}

			try
			{
				export.Exportuj(exportSettings.Prepisy);
				_result.IsConverted = true;
			}
			catch (Exception exception)
			{
				_result.Errors.Add(exception);
				return _result;
			}

			if (!settings.Debug)
			{
				if (File.Exists(docxToXmlOutput))
					File.Delete(docxToXmlOutput);
			}

			List<VersionInfoSkeleton> versions = settings.GetVersionList(_documentId);//TODO tady dodelat nacitani novych verzi.

			_currentVersionInfoSkeleton = versions.Last();


			WriteListChange(finalOutputFileName, versions, _currentVersionInfoSkeleton);
			File.Copy(finalOutputFileName, settings.OutputFilePath, true);
			_result.MetadataFilePath = GetConversionMetadataFileFullPath(settings.OutputFilePath);

			SplittingResult splittingResult = null;
			if (settings.SplitDocumentByPageBreaks)
			{
				splittingResult = SplitDocumentByPageBreaks(settings.OutputFilePath, fileNameWithoutExtension);
				if (!splittingResult.IsSplitted)
				{
					_result.IsConverted = false;
					_result.Errors.Add(new DocumentSplittingException("Vyskytla se chyba při rozdělení souboru podle hranice stran."));
				}
			}

			TableOfContentResult tocResult = null;
			ContentInfoBuilder tocBuilder = new ContentInfoBuilder();
			tocBuilder.XmlFile = settings.OutputFilePath;
			tocBuilder.StartingElement = "body";
			tocResult = tocBuilder.MakeTableOfContent();

			GenerateConversionMetadataFile(splittingResult, tocResult, documentType, settings.OutputFilePath);

			if (!settings.Debug)
			{
				try
				{
					Directory.Delete(settings.TempDirectoryPath, true);
				}
				catch (IOException exception)
				{
					Directory.Delete(settings.TempDirectoryPath, true);
				}
			}
			return _result;
		}

		/// <summary>
		/// Sets default values to properties with null value.
		/// It assumes that all conversion is made in one directory.
		/// </summary>
		/// <param name="settings"></param>
		private void ResolveDefaultSettingsValues(DocxToTeiConverterSettings settings)
		{
			if (settings == null) return;
			if (settings.InputFilePath == null) return;
			FileInfo fileInfo = new FileInfo(settings.InputFilePath);
			DirectoryInfo directoryInfo = new DirectoryInfo(fileInfo.DirectoryName);
			if (settings.OutputFilePath == null)
				settings.OutputFilePath = Path.Combine(directoryInfo.FullName, Path.GetFileNameWithoutExtension(fileInfo.FullName) +
					XmlExtension);
			if (settings.TempDirectoryPath == null)
			{
				string tempDirectory = Path.Combine(directoryInfo.FullName, "Temp");
				if (!Directory.Exists(tempDirectory))
					Directory.CreateDirectory(tempDirectory);
				settings.TempDirectoryPath = tempDirectory;
			}
		}

		private void GenerateConversionMetadataFile(SplittingResult splittingResult,
			string documentType,
			string finalOutputFileName)
		{
			GenerateConversionMetadataFile(splittingResult, new TableOfContentResult(), documentType, finalOutputFileName);
		}

		private void GenerateConversionMetadataFile(SplittingResult splittingResult, TableOfContentResult tableOfContentResult, string documentType, string finalOutputFileName)
		{

			XDocument metada = new XDocument();

			XDocument teiDocument = XDocument.Load(finalOutputFileName);

			metada.Add(new XElement(nsItj + "document",
				new XAttribute("doctype", documentType),
				new XAttribute("versionId", _currentVersionInfoSkeleton.Id),
				new XAttribute(nsXml + "lang", "cs"),
				new XAttribute("n", _documentId),
				new XAttribute("xmlns", nsTei),
				new XAttribute(XNamespace.Xmlns + "itj", nsItj),
				new XAttribute(XNamespace.Xmlns + "nlp", nsNlp),
				new XAttribute(XNamespace.Xmlns + "xml", XNamespace.Xml)
				)
				);

			XElement header = teiDocument.Descendants(nsTei + "teiHeader").FirstOrDefault();
			metada.Root.Add(header);

			XElement toc = GenerateToc(tableOfContentResult);
			XElement hws = GenerateHwList(tableOfContentResult);
			XElement hwt = GenerateHwTable(hws);


			if (splittingResult != null) //generovat pouze v případě, že k rozdělení na strany došlo
			{
				XElement pages = new XElement(nsItj + "pages",
					from info in splittingResult.PageBreaksSplitInfo
					select new XElement(nsItj + "page",
						info.Number == null ? null : new XAttribute("n", info.Number),
						info.Id == null ? null : new XAttribute(nsXml + "id", info.Id),
						info.FileName == null ? null : new XAttribute("resource", info.FileName),
						info.Facsimile == null ? null : new XAttribute("facs", info.Facsimile)
						)
					);

				metada.Root.Add(pages);
			}
			metada.Root.Add(toc);
			metada.Root.Add(hwt);
			metada.Root.Add(hws);
			metada.Save(GetConversionMetadataFileFullPath(finalOutputFileName));

		}


		private XElement GenerateToc(TableOfContentResult result)
		{
			XDocument doc = new XDocument(new XElement(nsItj + "tableOfContent"));
			doc.Root.Add(GenerateList(result.Sections));
			return doc.Root;
		}

		private XElement GenerateHwList(TableOfContentResult result)
		{
			XDocument doc = new XDocument(new XElement(nsItj + "headwordsList"));
			doc.Root.Add(GenerateList(result.HeadwordsList));
			return doc.Root;
		}

		private XElement GenerateHwTable(TableOfContentResult result)
		{
			XDocument doc = new XDocument(new XElement(nsItj + "headwordsTable"));
			doc.Root.Add(GenerateTable(result.HeadwordsList));
			return doc.Root;
		}

		private XElement GenerateHwTable(XElement result)
		{
			XDocument doc = new XDocument(new XElement(nsItj + "headwordsTable"));

			var items = from item in result.Descendants(nsTei + "item").Where(item => item.Element(nsTei + "list") == null)
									select new
									{
										EntryId = item.Parent.Parent.Attribute("corresp").Value.Replace("#", ""),
										DefaultHw = item.Parent.Parent.Element(nsTei + "head").Value,
										Headword = item.Element(nsTei + "head").Value,
										Visibility = item.Parent.Parent.Attribute("type") != null ? item.Parent.Parent.Attribute("type").Value : null,
										Type = item.Attribute("type") != null ? item.Attribute("type").Value : null
									};
			foreach (var item in items)
			{
				doc.Root.Add(new XElement(nsItj + "headword",
						new XAttribute("entryId", item.EntryId),
						new XAttribute("defaultHw", item.DefaultHw),
						new XAttribute("hw", item.Headword),
						item.Visibility != null ? new XAttribute("visibility", item.Visibility) : null,
						item.Type != null ? new XAttribute("type", item.Type) : null
						));
			}
			return doc.Root;
		}

		private XElement GenerateList(List<HeadwordsListItem> items)
		{
			if (items.Count > 0)
			{
				XElement list = new XElement(nsTei + "list");
				foreach (HeadwordsListItem item in items)
				{
					list.Add(GenerateList(item));
				}
				if (list.IsEmpty)
					return null;
				return list;
			}
			return null;
		}

		private IEnumerable<XElement> GenerateTable(List<HeadwordsListItem> items)
		{
			if (items.Count > 0)
			{

				var elements = from item in items
					where item.Sections.Count == 0
						select new
						{
							EntryId = item.Parent.DivXmlId,
							DefaultHw = item.Parent.HeadInfo.HeadText,
							Headword = item.HeadInfo.HeadText
						}
				;

				List<XElement> result = new List<XElement>();
				foreach (var element in elements)
				{
					result.Add(new XElement(nsItj + "headword",
						new XAttribute("entryId", element.EntryId),
						new XAttribute("defaultHw", element.DefaultHw),
						new XAttribute("hw", element.Headword)));
				}
				
				return result;
			}
			return null;
		}

		private XElement GenerateTable(List<ItemBase> items)
		{
			if (items.Count > 0)
			{
				XElement list = new XElement(nsTei + "list");
				foreach (ItemBase item in items)
				{
					if (item is TableOfContentItem)
					{
						//list.Add(GenerateTable(item as TableOfContentItem));
					}
					if (item is HeadwordsListItem)
					{
						list.Add(GenerateTable(item as HeadwordsListItem));
					}
				}
				if (list.IsEmpty)
					return null;
				return list;
			}
			return null;
		}

		private object GenerateTable(HeadwordsListItem item)
		{
			if (item == null) return null;
			string corresp = item.HeadwordInfo.FormXmlId ?? item.DivXmlId;
			XAttribute type = null;
			if (item.HeadwordInfo.Type != null)
				type = new XAttribute("type", item.HeadwordInfo.Type);
			XElement it =
				new XElement(nsTei + "item", new XAttribute("corresp", "#" + corresp),
					type,
					new XAttribute("defaulthw", item.HeadInfo.HeadText),
					item.PageBreakInfo.PageBreak == null
						? null
						: GetPbInfo(item.PageBreakInfo));
			return it;
			//GenerateTable(item.Sections)
		}

		private List<XAttribute> GetPbInfo(PageBreakInfo item)
		{
			return new List<XAttribute>()
			{
				new XAttribute("pb-id", item.PageBreakXmlId),
				new XAttribute("pb", item.PageBreak)
			};
		}

		private XElement GenerateList(List<ItemBase> items)
		{
			if (items.Count > 0)
			{
				XElement list = new XElement(nsTei + "list");
				foreach (ItemBase item in items)
				{
					if (item is TableOfContentItem)
					{
						list.Add(GenerateList(item as TableOfContentItem));
					}
					if (item is HeadwordsListItem)
					{
						list.Add(GenerateList(item as HeadwordsListItem));
					}
				}
				if (list.IsEmpty)
					return null;
				return list;
			}
			return null;
		}

		private XElement GenerateList(HeadwordsListItem item)
		{
			if (item == null) return null;
			string corresp = item.HeadwordInfo.FormXmlId ?? item.DivXmlId;
			XAttribute type = null;
			if (item.HeadwordInfo.Type != null)
				type = new XAttribute("type", item.HeadwordInfo.Type);
			XElement it =
				new XElement(nsTei + "item", new XAttribute("corresp", "#" + corresp),
					type,
					new XElement(nsTei + "head", new XText(item.HeadInfo.HeadText)),
					item.PageBreakInfo.PageBreak == null
						? null
						: new XElement(nsTei + "ref", new XAttribute("target", "#" + item.PageBreakInfo.PageBreakXmlId),
							new XText(item.PageBreakInfo.PageBreak)),
					GenerateList(item.Sections)
					);
			return it;
		}


		private  XElement GenerateList(List<TableOfContentItem> items)
		{
			if (items.Count > 0)
			{
				XElement list = new XElement(nsTei + "list");
				foreach (TableOfContentItem item in items)
				{
					list.Add(GenerateList(item));
				}
				if (list.IsEmpty)
					return null;
				return list;
			}
			return null;
		}

		private XElement GenerateList(TableOfContentItem item)
		{
			if (item == null || item.PageBreakInfo.PageBreakXmlId == null) return null;
			string corresp = item.DivXmlId;
			XElement it =
				new XElement(nsTei + "item", new XAttribute("corresp", "#" + corresp),
						new XElement(nsTei + "head", new XText(item.HeadInfo.HeadText)),
						item.PageBreakInfo.PageBreak == null ? null : new XElement(nsTei + "ref", new XAttribute("target", "#" + item.PageBreakInfo.PageBreakXmlId), new XText(item.PageBreakInfo.PageBreak)),
						GenerateList(item.Sections)
					);
			return it;
		}

		/*
		private XElement GenerateList(TableOfContentItem item)
		{
			if (item == null) return null;
			string corresp = item.FormXmlId ?? item.DivXmlId;
			XElement it =
				new XElement(nsTei + "item", new XAttribute("corresp", "#" + corresp), (item.Type == null) ? null : new XAttribute("type", item.Type),
						new XElement(nsTei + "head", new XText(item.Head)),
						item.PageBreak == null ? null : new XElement(nsTei + "ref", new XAttribute("target", "#" + item.PageBreakXmlId), new XText(item.PageBreak)),
						GenerateList(item.Sections)
					);
			return it;
		}
		*/

		private static string GetConversionMetadataFileFullPath(string outputFilePath)
		{
			string result = outputFilePath.Replace(XmlExtension, ".xmd");
			return result;
		}

		private SplittingResult SplitDocumentByPageBreaks(string fileFullPath, string fileNameWithoutExtension)
		{
			FileInfo file = new FileInfo(fileFullPath);
			//string outputDirectory = Path.Combine(file.DirectoryName, fileNameWithoutExtension);
			string outputDirectory = file.DirectoryName;

			if (!Directory.Exists(outputDirectory))
				Directory.CreateDirectory(outputDirectory);

			Splitter splitter = new Splitter(fileFullPath, outputDirectory);
			splitter.StartingElement = "body";

			SplittingResult splittingResult = splitter.SplitOnPageBreak();
			return splittingResult;
		}

		private static string GetDocumentType(string documentType)
		{
			switch (documentType)
			{
				case "edice":
					documentType = "Edition";
					break;
				case "slovník":
					documentType = "Dictionary";
					break;
				case "mluvnice":
					documentType = "Grammar";
					break;
				case "odborná literatura":
					documentType = "ProfessionalLiterature";
					break;
			}
			return documentType;
		}

		private ExportBase GetExportModule(string documentType, IExportNastaveni exportSettings)
		{
			switch (documentType)
			{
				case "Edition":
				case "ProfessionalLiterature":
					return new EdicniModul(exportSettings);
				case "Dictionary":
					return new SlovnikovyModul(exportSettings);
			}

			return null;
		}


		private static IExportNastaveni GetExportSettings(string documentType,
			DocxToTeiConverterSettings settings,
			string xsltTransformationsDirectoryPath,
			string xsltTemplatesDirectoryPath,
			AdresarovaStruktura ads,
			List<IPrepis> glsPrepisy)
		{
			string xsltTransformationFilePath = Path.Combine(xsltTransformationsDirectoryPath, documentType + XmlExtension);
			IExportNastaveni exportSettings = null;
			switch (documentType)
			{
				case "Edition":
				case "ProfessionalLiterature":
					exportSettings = GetEdicniModulNastaveni(settings, xsltTransformationFilePath, xsltTemplatesDirectoryPath, ads, glsPrepisy);
					break;
				case "Dictionary":
					exportSettings = GetDictionarySettings(settings, xsltTransformationFilePath, xsltTemplatesDirectoryPath, ads,
						glsPrepisy);
					break;
			}
			return exportSettings;
		}
		private static IExportNastaveni GetEdicniModulNastaveni(DocxToTeiConverterSettings settings,
			string xsltTransformationFilePath,
			string xsltTemplatesPath,
			AdresarovaStruktura ads, List<IPrepis> glsPrepisy)
		{
			IExportNastaveni nastaveni = new EdicniModulNastaveni();
			nastaveni.SouborTransformaci = xsltTransformationFilePath;
			nastaveni.SlozkaXslt = xsltTemplatesPath;
			nastaveni.VstupniSlozka = ads.DejSpolecneDocXml;
			nastaveni.VystupniSlozka = ads.DejVystup;
			nastaveni.DocasnaSlozka = ads.DejTemp;
			nastaveni.Prepisy = glsPrepisy;
			nastaveni.SmazatDocasneSoubory = !settings.Debug;

			nastaveni.SouborMetadat = settings.MetadataFilePath;
			return nastaveni;
		}

		private static IExportNastaveni GetDictionarySettings(DocxToTeiConverterSettings settings,
			string xsltTransformationFilePath,
			string xsltTemplatesPath,
			AdresarovaStruktura ads, List<IPrepis> glsPrepisy)
		{
			IExportNastaveni nastaveni = new SlovnikovyModulNastaveni();
			nastaveni.SouborTransformaci = xsltTransformationFilePath;
			nastaveni.SlozkaXslt = xsltTemplatesPath;
			nastaveni.VstupniSlozka = ads.DejSpolecneDocXml;
			nastaveni.VystupniSlozka = ads.DejVystup;
			nastaveni.DocasnaSlozka = ads.DejTemp;
			nastaveni.Prepisy = glsPrepisy;
			nastaveni.SmazatDocasneSoubory = !settings.Debug;

			nastaveni.SouborMetadat = settings.MetadataFilePath;
			return nastaveni;
		}

		private void WriteListChange(string docxToXmlOutput, IEnumerable<VersionInfoSkeleton> previousVersions, VersionInfoSkeleton currentVersionInfoSkeleton)
		{
			XNamespace tei = "http://www.tei-c.org/ns/1.0";
			XDocument document = XDocument.Load(docxToXmlOutput);

			XElement revisionElement = document.Element(tei + "TEI").Element(tei + "teiHeader").Element(tei + "revisionDesc");
			if (revisionElement == null)
			{
				XElement teiHeader = document.Element(tei + "TEI").Element(tei + "teiHeader");
				revisionElement = new XElement(tei + "revisionDesc");
				foreach (VersionInfoSkeleton version in previousVersions)
				{
					revisionElement.Add(new XElement(tei + "change", new XAttribute("n", version.Id), new XAttribute("when", version.Creation), new XText(version.Message ?? "")));
				}
				teiHeader.Add(revisionElement);
			}
			document.Root.Add(new XAttribute("change", "#" + currentVersionInfoSkeleton.Id));
			document.Save(docxToXmlOutput);
		}

		private static List<IPrepis> GetPrepisy(DocxToTeiConverterSettings settings, string inputFileName)
		{
			List<IPrepis> glsPrepisy = new List<IPrepis>();
			Prepisy prepisy = Perzistence.NacistZXml(settings.MetadataFilePath);
			Prepis prepis = (from p in prepisy where p.Soubor.Nazev == inputFileName select p).FirstOrDefault();
			glsPrepisy.Add(prepis);
			return glsPrepisy;
		}

		private void ConvertDocxToXml(List<IPrepis> prepisy, string outputDirectory, string docxToXmlFilePath)
		{
			foreach (Prepis prepis in prepisy)
			{
				//FileInfo fi = new FileInfo(Path.Combine(outputDirectory, prepis.Soubor.NazevBezPripony + ".xml"));
				ConvertDocxToXml(prepis.Soubor.CelaCesta, docxToXmlFilePath, outputDirectory, prepis.Soubor.NazevBezPripony, DateTime.UtcNow);
			}
		}

		private static void ConvertDocxToXml(string inputDocxFile, string docxToXmlFilePath,
			string outputDirectory, string fileNameWithoutExtension, DateTime exportTime)
		{
			Settings stg = new Settings();
			string souborXml = Path.Combine(outputDirectory, fileNameWithoutExtension + ".xml");
			XmlGenerator xg = new XmlGenerator(inputDocxFile, docxToXmlFilePath, souborXml, stg);
			xg.Read();

			File.SetCreationTime(souborXml, exportTime);

		}

		private static void ConvertDocxToXml(string inputFile, string docxToXmlFilePath, string outputFile)
		{
			Settings settings = new Settings();
			XmlGenerator xg = new XmlGenerator(inputFile, docxToXmlFilePath, outputFile, settings);
			xg.Read();

		}

		private static string GetDataDirectoryPath()
		{
			string directory = AssemblyDirectory;
			//cstrSablonyXslt = cstrSablonyXslt.Substring(0, cstrSablonyXslt.LastIndexOf(Path.DirectorySeparatorChar));
			directory = Path.GetFullPath(Path.Combine(directory, @"Data"));
			return directory;
		}

		private static string GetXsltTemplatesPath()
		{
			string cstrSablonyXslt = GetDataDirectoryPath();
			//cstrSablonyXslt = cstrSablonyXslt.Substring(0, cstrSablonyXslt.LastIndexOf(Path.DirectorySeparatorChar));
			cstrSablonyXslt = Path.Combine(cstrSablonyXslt, "Xslt");
			return cstrSablonyXslt;
		}


		/// <summary>
		/// Path to diractory with XSLT transformations definition files.
		/// </summary>
		/// <returns></returns>
		private static string GetXsltTransformationsPath()
		{
			string directory = GetDataDirectoryPath();
			//cstrSablonyXslt = cstrSablonyXslt.Substring(0, cstrSablonyXslt.LastIndexOf(Path.DirectorySeparatorChar));
			directory = Path.Combine(directory, @"Transformations");
			return directory;
		}


		public static string AssemblyDirectory
		{
			get
			{
				string codeBase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}
	}


}
