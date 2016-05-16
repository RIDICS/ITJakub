using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Daliboris.OOXML.Word.Transform;
using Daliboris.Texty.Evidence;
using Daliboris.Texty.Evidence.Rozhrani;
using Daliboris.Texty.Export;
using Daliboris.Texty.Export.Rozhrani;
using Daliboris.Texty.Export.SlovnikovyModul;
using Ujc.Ovj.Xml.Tei.Contents;
using Ujc.Ovj.Xml.Tei.Splitting;

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

	    private DocxToTeiConverterSettings ConverterSettings { get; set; }

		public string GetDocumentId()
		{
			return _documentId;
		}

	    private string GetDocxToXmlOutput(AdresarovaStruktura ads, string fileNameWithoutExtension, int filePart, bool useMultipleName)
	    {
	        return useMultipleName
	            ? Path.Combine(ads.DejSpolecneDocXml, String.Format("{0}_source_{1:00}{2}", fileNameWithoutExtension, filePart, XmlExtension))
	            : Path.Combine(ads.DejSpolecneDocXml, String.Format("{0}{1}", fileNameWithoutExtension, XmlExtension));
        }


        public ConversionResult Convert(DocxToTeiConverterSettings settings)
		{
		    ConverterSettings = settings;
		    CheckIfDirectoryPathsExists(ConverterSettings);
			_result = new ConversionResult();
			string documentType = null;

            //get metadata only for first (by alphabet) uploaded file
            var inputFileName= ConverterSettings.InputFilesPath.Select(filePath => new FileInfo(filePath)).Select(fileInfo => fileInfo.Name).First();

		    ResolveDefaultSettingsValues(ConverterSettings);

            var prepis = GetPrepisy(ConverterSettings, inputFileName);

			if (prepis == null)
			{
				//dokument v evidenci neexistuje, nabídnout zanesení dokumentu do evidence
				// mělo by stačit přiřazení typu dokumentu
				_result.Errors.Add(new DocumentNotInEvidenceException(String.Format("Dokument s uvedeným jménem souboru '{0}' neexistuje v evidenci.", inputFileName)));
				return _result;
			}
            
		    if (prepis.FazeZpracovani < FazeZpracovani.Exportovat)
		    {
                _result.Errors.Add(new DocumentNotInRequredStateException("Dokument s uvedeným jménem souboru není připraven pro export."));
                return _result;
            }

			documentType = GetDocumentType(prepis.TypPrepisu);
			_documentId = prepis.GUID;

			if (documentType == null)
			{
				//dokument má v evidenci přiřazen typ dokumentu, který není podporován
				_result.Errors.Add(new NotSupportedFileFormatException("Dokument má v evidenci přiřazen typ dokumentu, který není podporován."));
				return _result;
			}

            string tempDirectoryPath = ConverterSettings.TempDirectoryPath;
			//vytvoří se adresářová struktura, pokud neexistuje, pro ukládání výsledných a dočasných souborů
			AdresarovaStruktura ads = new AdresarovaStruktura(tempDirectoryPath, documentType);
			ads.VytvorStrukturu();


			string docxToXmlFilePath = Path.Combine(GetDataDirectoryPath(), "AllStylesConvert.2xml");
			string xsltTemplatesPath = GetXsltTemplatesPath();
			string xsltTransformationsPath = GetXsltTransformationsPath();
            
			string fileNameWithoutExtension = prepis.Soubor.NazevBezPripony;
			string xmlOutpuFileName = fileNameWithoutExtension + XmlExtension;
            
			string finalOutputDirectory = ads.DejVystup; // Path.Combine(ads.DejVystup, fileNameWithoutExtension);
			string finalOutputFileName = Path.Combine(finalOutputDirectory, xmlOutpuFileName);

			//Zatím pouze konverze z DOCX do základního XML
            IList<string> xmlOutputFiles=new List<string>();
			try
			{
			    var filePart = 0;
			    foreach (var inputFilePath in ConverterSettings.InputFilesPath)
			    {
			        xmlOutputFiles.Add(GetDocxToXmlOutput(ads, prepis.Soubor.NazevBezPripony, filePart, ConverterSettings.InputFilesPath.Length > 1));

                    ConvertDocxToXml(inputFilePath, docxToXmlFilePath, xmlOutputFiles.Last());
			        filePart++;
			    }
			}
			catch (Exception exception)
			{
				_result.Errors.Add(exception);
				return _result;
			}

			if (!Directory.Exists(finalOutputDirectory))
				Directory.CreateDirectory(finalOutputDirectory);

            IExportNastaveni exportSettings = GetExportSettings(documentType, ConverterSettings, xsltTransformationsPath, xsltTemplatesPath, ads, prepis);
			ExportBase export = GetExportModule(documentType, exportSettings, xmlOutputFiles);

			if (export == null || exportSettings == null)
			{
				//Objekt pro export se nepodažřilo vytvořit, není podporován.
				return _result;
			}

			try
			{
				export.Exportuj(exportSettings.Prepis, xmlOutputFiles);
				_result.IsConverted = true;
			}
			catch (Exception exception)
			{
				_result.Errors.Add(exception);
				return _result;
			}

			if (!settings.Debug)
			{
			    foreach (var xmlOutputFile in xmlOutputFiles)
			    {    
				if (File.Exists(xmlOutputFile))
					File.Delete(xmlOutputFile);
			    }
			}

			List<VersionInfoSkeleton> versions = settings.GetVersionList(_documentId);

			_currentVersionInfoSkeleton = versions.Last();


			WriteListChange(finalOutputFileName, versions, _currentVersionInfoSkeleton);
		    string xmlFinalOutputPath = Path.Combine(settings.OutputDirectoryPath, xmlOutpuFileName);



            File.Copy(finalOutputFileName, xmlFinalOutputPath, true);
			_result.MetadataFilePath = settings.OutputMetadataFilePath;
                //GetConversionMetadataFileFullPath(settings.OutputFilePath);

			SplittingResult splittingResult = null;
		    if (settings.SplitDocumentByPageBreaks)
			{
				splittingResult = SplitDocumentByPageBreaks(xmlFinalOutputPath, fileNameWithoutExtension);
				if (!splittingResult.IsSplitted)
				{
					_result.IsConverted = false;
					_result.Errors.Add(new DocumentSplittingException("Vyskytla se chyba při rozdělení souboru podle hranice stran."));
				}
			}

			TableOfContentResult tocResult = null;
			ContentInfoBuilder tocBuilder = new ContentInfoBuilder();
			tocResult = tocBuilder.MakeTableOfContent(xmlFinalOutputPath, "body");

			GenerateConversionMetadataFile(splittingResult, tocResult, documentType, 
                xmlFinalOutputPath, xmlOutpuFileName, settings.OutputMetadataFilePath);

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

	    private void CheckIfDirectoryPathsExists(DocxToTeiConverterSettings converterSettings)
	    {
	        if(!Directory.Exists(converterSettings.DataDirectoryPath))
                throw new DirectoryNotFoundException(String.Format("Složka s daty neexistuje. Složka: {0}", converterSettings.DataDirectoryPath));
	        string xsltTemplatesPath = GetXsltTemplatesPath();
	        string xsltTransformationsPath = GetXsltTransformationsPath();
            if(!Directory.Exists(xsltTemplatesPath))
                throw new DirectoryNotFoundException(String.Format("Složka s šablonami XSLT neexistuje. Složka: {0}", xsltTemplatesPath));
            if (!Directory.Exists(xsltTransformationsPath))
                throw new DirectoryNotFoundException(String.Format("Složka s transformacemi děl neexistuje. Složka: {0}", xsltTransformationsPath));
        }

	    /// <summary>
		/// Sets default values to properties with null value.
		/// It assumes that all conversion is made in one directory.
		/// </summary>
		/// <param name="settings"></param>
		private void ResolveDefaultSettingsValues(DocxToTeiConverterSettings settings)
		{
	        var inputFilePath = settings?.InputFilesPath.First();

            if (inputFilePath == null) return;

			var fileInfo = new FileInfo(inputFilePath);
	        if (fileInfo.DirectoryName != null)
	        {
	            var directoryInfo = new DirectoryInfo(fileInfo.DirectoryName);
	            //if (settings.OutputFilePath == null)
	            //	settings.OutputFilePath = Path.Combine(directoryInfo.FullName, Path.GetFileNameWithoutExtension(fileInfo.FullName) +
	            //		XmlExtension);
	            if (settings.TempDirectoryPath != null) return;

	            var tempDirectory = Path.Combine(directoryInfo.FullName, "Temp");
	            if (!Directory.Exists(tempDirectory))
	                Directory.CreateDirectory(tempDirectory);
	            settings.TempDirectoryPath = tempDirectory;
	        }
	        else
	        {
	            throw new Exception("Not exist filepath");
	        }
		}

		//private void GenerateConversionMetadataFile(SplittingResult splittingResult,
		//	string documentType,
		//	string finalOutputFileFullPath)
		//{
		//	GenerateConversionMetadataFile(splittingResult, new TableOfContentResult(), documentType, finalOutputFileFullPath);
		//}

		private void GenerateConversionMetadataFile(SplittingResult splittingResult, TableOfContentResult tableOfContentResult, 
            string documentType, string finalOutputFileFullPath, string finalOutputFileName, string finalOutputMetadataFileName)
		{

			XDocument metada = new XDocument();

			XDocument teiDocument = XDocument.Load(finalOutputFileFullPath);

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
            XElement accessories = new XElement(nsItj + "accessories", 
                new XElement(nsItj + "file", new XAttribute("type", "content"), new XAttribute("name", finalOutputFileName)));

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
            metada.Root.Add(accessories);
			metada.Save(finalOutputMetadataFileName);

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

			var items = from item in result.Descendants(nsTei + "item").Where(
			    item =>
			    {
			        return item.Element(nsTei + "list") == null;
			    }
                )
									select new
									{
										EntryId = item.Parent.Parent.Attribute("corresp").Value.Replace("#", ""),
										DefaultHw = item.Parent.Parent.Element(nsTei + "head").Value,
                                        DefaultHwSorting = (from i in item.Parent.Parent.Element(nsTei + "head").ElementsAfterSelf(nsTei + "interp") where i.Attribute("type").Value == "sorting" select i).FirstOrDefault(),
										Headword = item.Element(nsTei + "head").Value,
                                        HeadwordSorting = (from i in item.Element(nsTei + "head").ElementsAfterSelf(nsTei + "interp") where i.Attribute("type").Value == "sorting" select i).FirstOrDefault(), 
										Visibility = item.Parent.Parent.Attribute("type") != null ? item.Parent.Parent.Attribute("type").Value : null,
										Type = item.Attribute("type") != null ? item.Attribute("type").Value : null
									};
			foreach (var item in items)
			{
				doc.Root.Add(new XElement(nsItj + "headword",
						new XAttribute("entryId", item.EntryId),
						new XAttribute("defaultHw", item.DefaultHw),
                        item.DefaultHwSorting != null ? new XAttribute("defaultHw-sorting", item.DefaultHwSorting.Value) : null,
                        item.HeadwordSorting != null ? new XAttribute("hw", item.DefaultHwSorting.Value) : null, 
                        item.Headword != null ? new XAttribute("hw-original", item.Headword) : null,
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
                    new XElement(nsTei + "interp", new XAttribute("type", "sorting"), new XText(item.HeadInfo.HeadSort())),
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

		//private static string GetConversionMetadataFileFullPath(string outputFilePath)
		//{
		//	string result = outputFilePath.Replace(XmlExtension, ".xmd");
		//	return result;
		//}

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

		private ExportBase GetExportModule(string documentType, IExportNastaveni exportSettings, IList<string> xmlOutputFiles)
		{
			switch (documentType)
			{
				case "Edition":
				case "ProfessionalLiterature":
					return new EdicniModul(exportSettings, xmlOutputFiles);
				case "Dictionary":
					return new SlovnikovyModul(exportSettings, xmlOutputFiles);
                case "Grammar":
			        return new ModulMluvnic(exportSettings, xmlOutputFiles);
			}

			return null;
		}


		private static IExportNastaveni GetExportSettings(string documentType,
			DocxToTeiConverterSettings settings,
			string xsltTransformationsDirectoryPath,
			string xsltTemplatesDirectoryPath,
			AdresarovaStruktura ads,
			IPrepis prepis)
		{
			string xsltTransformationFilePath = Path.Combine(xsltTransformationsDirectoryPath, documentType + XmlExtension);
			IExportNastaveni exportSettings = null;
			switch (documentType)
			{
				case "Edition":
				case "ProfessionalLiterature":
					exportSettings = GetEdicniModulNastaveni(settings, xsltTransformationFilePath, xsltTemplatesDirectoryPath, ads, prepis);
					break;
				case "Dictionary":
					exportSettings = GetDictionarySettings(settings, xsltTransformationFilePath, xsltTemplatesDirectoryPath, ads,
                        prepis);
					break;
			}
			return exportSettings;
		}
		private static IExportNastaveni GetEdicniModulNastaveni(DocxToTeiConverterSettings settings,
			string xsltTransformationFilePath,
			string xsltTemplatesPath,
			AdresarovaStruktura ads, IPrepis prepis)
		{
			IExportNastaveni nastaveni = new EdicniModulNastaveni();
			nastaveni.SouborTransformaci = xsltTransformationFilePath;
			nastaveni.SlozkaXslt = xsltTemplatesPath;
			nastaveni.VstupniSlozka = ads.DejSpolecneDocXml;
			nastaveni.VystupniSlozka = ads.DejVystup;
			nastaveni.DocasnaSlozka = ads.DejTemp;
            nastaveni.Prepis = prepis;
            nastaveni.SmazatDocasneSoubory = !settings.Debug;

			nastaveni.SouborMetadat = settings.MetadataFilePath;
			return nastaveni;
		}

		private static IExportNastaveni GetDictionarySettings(DocxToTeiConverterSettings settings,
			string xsltTransformationFilePath,
			string xsltTemplatesPath,
			AdresarovaStruktura ads, IPrepis prepis)
		{
			IExportNastaveni nastaveni = new SlovnikovyModulNastaveni();
			nastaveni.SouborTransformaci = xsltTransformationFilePath;
			nastaveni.SlozkaXslt = xsltTemplatesPath;
			nastaveni.VstupniSlozka = ads.DejSpolecneDocXml;
			nastaveni.VystupniSlozka = ads.DejVystup;
			nastaveni.DocasnaSlozka = ads.DejTemp;
			nastaveni.Prepis = prepis;
            nastaveni.SmazatDocasneSoubory = !settings.Debug;

			nastaveni.SouborMetadat = settings.MetadataFilePath;
			return nastaveni;
		}

		private void WriteListChange(string docxToXmlOutput, IEnumerable<VersionInfoSkeleton> previousVersions, VersionInfoSkeleton currentVersionInfoSkeleton)
		{
		    XNamespace tei = XNamespace.Get("http://www.tei-c.org/ns/1.0");
            //"http://www.tei-c.org/ns/1.0";

            XDocument document = XDocument.Load(docxToXmlOutput);

            //document.Root.Attribute("xmlns:tei")

            XElement teiHeader = document.Element(tei + "TEI").Element(tei + "teiHeader");
			XElement revisionElement = teiHeader.Element(tei + "revisionDesc");

            if (revisionElement == null)
			{
				revisionElement = new XElement(tei + "revisionDesc");
				foreach (VersionInfoSkeleton version in previousVersions)
				{
					revisionElement.Add(new XElement(tei + "change", new XAttribute("n", version.Id), new XAttribute("when", version.Creation), new XText(version.Message ?? "")));
				}
				teiHeader.Add(revisionElement);
			}
		    var teiN = document.Root.Attribute("n");

            if (teiN==null)
            {
                var fileDesc = teiHeader.Element(tei+ "fileDesc");
                document.Root.Add(new XAttribute("n", fileDesc.Attribute("n").Value));
            }

            document.Root.Add(new XAttribute("change", "#" + currentVersionInfoSkeleton.Id));
			document.Save(docxToXmlOutput);
		}

		private static IPrepis GetPrepisy(DocxToTeiConverterSettings settings, string inputFileName)
		{
            var searchedFileName = inputFileName;
            var extensionDotPosition = inputFileName.LastIndexOf('.');
            if (extensionDotPosition > 0)
            {
                searchedFileName = String.Format("{0}.{1}",
                    inputFileName.Substring(0, extensionDotPosition).Split('_').First(),
                    inputFileName.Substring(extensionDotPosition + 1, inputFileName.Length - extensionDotPosition - 1));
            }
            
			var prepisy = Perzistence.NacistZXml(settings.MetadataFilePath);
			var prepis = prepisy.FirstOrDefault(p => p.Soubor.Nazev == inputFileName) ??
			             prepisy.FirstOrDefault(p =>p.Soubor.Nazev == searchedFileName);
            
			return prepis;
		}

		private void ConvertDocxToXml(IPrepis prepis, string outputDirectory, string docxToXmlFilePath)
		{
			//FileInfo fi = new FileInfo(Path.Combine(outputDirectory, prepis.Soubor.NazevBezPripony + ".xml"));
			ConvertDocxToXml(prepis.Soubor.CelaCesta, docxToXmlFilePath, outputDirectory, prepis.Soubor.NazevBezPripony, DateTime.UtcNow);
		}

		private static void ConvertDocxToXml(string inputDocxFile, string docxToXmlFilePath,
			string outputDirectory, string fileNameWithoutExtension, DateTime exportTime)
		{
			string souborXml = Path.Combine(outputDirectory, fileNameWithoutExtension + ".xml");

		    ConvertDocxToXml(inputDocxFile, docxToXmlFilePath, souborXml);
            
			File.SetCreationTime(souborXml, exportTime);

		}

		private static void ConvertDocxToXml(string inputFile, string docxToXmlFilePath, string outputFile)
		{
			Settings settings = new Settings();
			XmlGenerator xg = new XmlGenerator(inputFile, docxToXmlFilePath, outputFile, settings);
			xg.Read();

		}

		private string GetDataDirectoryPath()
		{
			string directory = AssemblyDirectory;
			//cstrSablonyXslt = cstrSablonyXslt.Substring(0, cstrSablonyXslt.LastIndexOf(Path.DirectorySeparatorChar));
            //directory = Path.GetFullPath(Path.Combine(directory, @"Data"));
            directory = ConverterSettings.DataDirectoryPath;
			return directory;
		}

		private string GetXsltTemplatesPath()
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
		private string GetXsltTransformationsPath()
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
