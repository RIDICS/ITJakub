using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.Mime;
using System.Xml;
using Daliboris.OOXML.Word;


namespace Daliboris.OOXML.Pomucky
{
	public class Dokument
	{
		public const string RelDocumentRelationshipType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";
		public const string RelStylesRelationshipType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles";
		public const string RelCorePropertiesType = "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties";
		public const string RelDocumentRelationshipsType = "http://schemas.openxmlformats.org/package/2006/relationships";
		public const string RelExtendedPropertiesType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties";
		public const string RelFootnotesRelationshipType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footnotes";
		public const string RelCustomization = "http://schemas.microsoft.com/office/word/2006/wordml";
		public const string RelCustomUiRelationshipType = "http://schemas.microsoft.com/office/2006/relationships/ui/extensibility";
		public const string RelCustomUiW14RelationshipType = "http://schemas.microsoft.com/office/2007/relationships/ui/extensibility";
		public const string RelWordprocessingRelationshipTypeW = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
		public const string RelWordprocessingRelationshipTypeW14 = "http://schemas.microsoft.com/office/word/2010/wordml";


		public static void ExtrahovatDoSouboru(string strDocx, string strVystup)
		{
			ExtrahovatDoSouboru(strDocx, strVystup, false);
		}

		public static void ExtrahovatStylyDoSouboru(string strDocx, string strVystup)
		{
			ExtrahovatStylyDoSouboru(strDocx, strVystup, false);
		}


		public static void ExtrahovatRozsireneVlastnostiDoSouboru(string strDocx, string strVystup)
		{
			ExtrahovatRozsireneVlastnostiDoSouboru(strDocx, strVystup, false);
		}
		public static void ExtrahovatRozsireneVlastnostiDoSouboru(string strDocx, string strVystup, bool blnFormatovatOdsazeni)
		{
			ExtrahovatVlastnostiDoSouboru(strDocx, strVystup, blnFormatovatOdsazeni, RelExtendedPropertiesType);
		}

		public static void ExtrahovatVlastnostiDoSouboru(string strDocx, string strVystup)
		{
			ExtrahovatVlastnostiDoSouboru(strDocx, strVystup, false);
		}
		public static void ExtrahovatVlastnostiDoSouboru(string strDocx, string strVystup, bool blnFormatovatOdsazeni)
		{
			ExtrahovatVlastnostiDoSouboru(strDocx, strVystup, blnFormatovatOdsazeni, RelCorePropertiesType);
		}

		private static void ExtrahovatVlastnostiDoSouboru(string strDocx, string strVystup, bool blnFormatovatOdsazeni,
			string strNsVlasnosti)
		{
			using (FileStream fs = new FileStream(strDocx, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (StreamReader sr = new StreamReader(fs))
				{
					using (Package wdPackage = Package.Open(sr.BaseStream))
					{

						{
							PackageRelationship docPackageRelationship = wdPackage.GetRelationshipsByType(strNsVlasnosti).FirstOrDefault();
							if (docPackageRelationship != null)
							{
								Uri documentUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), docPackageRelationship.TargetUri);
								PackagePart documentPart = wdPackage.GetPart(documentUri);
								UlozitPackagePart(strVystup, blnFormatovatOdsazeni, documentPart);
							}
						}
					}
				}
			}
		}

		public static void ExtrahovatStylyDoSouboru(string strDocx, string strVystup, bool blnFormatovatOdsazeni)
		{
			if (!File.Exists(strDocx))
				throw new FileNotFoundException("Soubor '" + strDocx + "' nebyl nalezen.");

			using (FileStream fs = new FileStream(strDocx, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (StreamReader sr = new StreamReader(fs))
				{
					using (Package wdPackage = Package.Open(sr.BaseStream))
					{
						PackageRelationship docPackageRelationship = wdPackage.GetRelationshipsByType(RelDocumentRelationshipType).FirstOrDefault();
						if (docPackageRelationship != null)
						{
							Uri documentUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), docPackageRelationship.TargetUri);
							PackagePart documentPart = wdPackage.GetPart(documentUri);

							//  Find the styles part. There will only be one.

							//chyba typu "Invalid URI: The hostname could not be parsed.", "Neplatný identifikátor URI. Nelze analyzovat název hostitele."
							//dokument obsahuje pole typu HYPERLINK  neplatnými adresami
							//http://social.msdn.microsoft.com/Forums/en-US/oxmlsdk/thread/de46d25a-2bee-47d6-a4c7-0974a7036200
							PackageRelationship styleRelation = documentPart.GetRelationshipsByType(RelStylesRelationshipType).FirstOrDefault();
							if (styleRelation != null)
							{
								Uri styleUri = PackUriHelper.ResolvePartUri(documentUri, styleRelation.TargetUri);
								PackagePart stylePart = wdPackage.GetPart(styleUri);
								UlozitPackagePart(strVystup, blnFormatovatOdsazeni, stylePart);
							}
						}
					}
				}
			}
			/*
			using (Package wdPackage = Package.Open(strDocx, FileMode.Open, FileAccess.Read))
			{
				PackageRelationship docPackageRelationship = wdPackage.GetRelationshipsByType(RelDocumentRelationshipType).FirstOrDefault();
				if (docPackageRelationship != null)
				{
					Uri documentUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), docPackageRelationship.TargetUri);
					PackagePart documentPart = wdPackage.GetPart(documentUri);

					//  Find the styles part. There will only be one.

					//chyba typu "Invalid URI: The hostname could not be parsed.", "Neplatný identifikátor URI. Nelze analyzovat název hostitele."
					//dokument obsahuje pole typu HYPERLINK  neplatnými adresami
					//http://social.msdn.microsoft.com/Forums/en-US/oxmlsdk/thread/de46d25a-2bee-47d6-a4c7-0974a7036200
					PackageRelationship styleRelation = documentPart.GetRelationshipsByType(RelStylesRelationshipType).FirstOrDefault();
					if (styleRelation != null)
					{
						Uri styleUri = PackUriHelper.ResolvePartUri(documentUri, styleRelation.TargetUri);
						PackagePart stylePart = wdPackage.GetPart(styleUri);
						UlozitPackagePart(strVystup, blnFormatovatOdsazeni, stylePart);
					}
				}
			}
			*/

		}

		public static void PrekopirovatPanelNastroju(string strZdrojovySoubor, string strCilovySoubor)
		{
			using (Package wdPackage = Package.Open(strCilovySoubor, FileMode.Open, FileAccess.ReadWrite))
			{
				PackageRelationship docPackageRelationship = wdPackage.GetRelationshipsByType(RelDocumentRelationshipType).FirstOrDefault();
				if (docPackageRelationship != null)
				{
					Uri documentUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), docPackageRelationship.TargetUri);
					PackagePart documentPart = wdPackage.GetPart(documentUri);

					//  Find the styles part. There will only be one.
					PackageRelationship styleRelation = documentPart.GetRelationshipsByType(RelCustomization).FirstOrDefault();
					if (styleRelation != null)
					{
						Uri styleUri = PackUriHelper.ResolvePartUri(documentUri, styleRelation.TargetUri);
						PackagePart stylePart = wdPackage.GetPart(styleUri);
						using (StreamReader streamReader = new StreamReader(strZdrojovySoubor))
						using (StreamWriter streamWriter = new StreamWriter(stylePart.GetStream(FileMode.Create)))
						{
							streamWriter.Write(streamReader.ReadToEnd());
						}
					}
				}
			}
		}


		/// <summary>
		/// Nahradí text dokumentu novým obsahem.
		/// </summary>
		/// <param name="souborDocx">Dokument DOCX, jehož obsah se má nahrazovat.</param>
		/// <param name="dokumentXml">Text dokumentu ve formátu Xml.</param>
		/// <param name="zalohovat">Zda se má dokument před provedením změn zálohovat.</param>
		/// <returns></returns>
		public static bool NahraditDokument(string souborDocx, string dokumentXml, bool zalohovat)
		{
			bool blnNahrazeno = false;

			ZalohovatDokument(souborDocx, zalohovat);

			using (Package wdPackage = Package.Open(souborDocx, FileMode.Open, FileAccess.ReadWrite))
			{
				PackageRelationship docPackageRelationship = wdPackage.GetRelationshipsByType(RelDocumentRelationshipType).FirstOrDefault();
				if (docPackageRelationship != null)
				{
					Uri documentUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), docPackageRelationship.TargetUri);
					PackagePart documentPart = wdPackage.GetPart(documentUri);

					using (StreamReader streamReader = new StreamReader(dokumentXml))
					using (StreamWriter streamWriter = new StreamWriter(documentPart.GetStream(FileMode.Create)))
					{
						streamWriter.Write(streamReader.ReadToEnd());
					}

					blnNahrazeno = true;
				}
			}
			return blnNahrazeno;
		}

		public static bool NahraditDefiniciStylu(string strDocx, string strStylyXml)
		{
			bool blnNahrazeno = false;
			using (Package wdPackage = Package.Open(strDocx, FileMode.Open, FileAccess.ReadWrite))
			{
				PackageRelationship docPackageRelationship = wdPackage.GetRelationshipsByType(RelDocumentRelationshipType).FirstOrDefault();
				if (docPackageRelationship != null)
				{
					Uri documentUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), docPackageRelationship.TargetUri);
					PackagePart documentPart = wdPackage.GetPart(documentUri);

					//  Find the styles part. There will only be one.
					PackageRelationship styleRelation = documentPart.GetRelationshipsByType(RelStylesRelationshipType).FirstOrDefault();
					if (styleRelation != null)
					{
						Uri styleUri = PackUriHelper.ResolvePartUri(documentUri, styleRelation.TargetUri);
						PackagePart stylePart = wdPackage.GetPart(styleUri);
						using (StreamReader streamReader = new StreamReader(strStylyXml))
						using (StreamWriter streamWriter = new StreamWriter(stylePart.GetStream(FileMode.Create)))
						{
							streamWriter.Write(streamReader.ReadToEnd());
						}
						blnNahrazeno = true;
					}
				}
			}
			return blnNahrazeno;
		}

		/// <summary>
		/// Vytvoří v dokumentu DOCX (případně DOTX)
		/// </summary>
		/// <param name="souborDocx"></param>
		/// <param name="customUiXml"></param>
		/// <returns></returns>
		public static bool VytvoritCustomUI(string souborDocx, string customUiXml)
		{
			return VytvoritCustomUI(souborDocx, customUiXml, true);
		}

		private static string ZjistiVerziCustomUi(string customUiXml)
		{
			string verze = null;
			XmlDocument xml = new XmlDocument();
			xml.Load(customUiXml);
			string att = xml.DocumentElement.GetAttribute("xmlns");
			switch (att)
			{
				case "http://schemas.microsoft.com/office/2009/07/customui":
					verze = RelCustomUiW14RelationshipType;
					break;
				case "http://schemas.microsoft.com/office/2006/01/customui":
					verze = RelCustomUiRelationshipType;
					break;
			}
			return verze;
		}

		public static bool VytvoritCustomUI(string souborDocx, string customUiXml, bool zalohovat)
		{
			bool uspech = false;

			ZalohovatDokument(souborDocx, zalohovat);

			string customUiRelationshipType = ZjistiVerziCustomUi(customUiXml);
			if (customUiRelationshipType == null)
				return uspech;

			string customUiXmlUri = "/customUI/customUI.xml";
			string CustomUiId = "rCustomUiId";
			if (customUiRelationshipType == RelCustomUiW14RelationshipType)
			{
				customUiXmlUri = "/customUI/customUI2010.xml";
				CustomUiId = "rCustomUi2010Id";
			}

			using (Package wdPackage = Package.Open(souborDocx, FileMode.Open, FileAccess.ReadWrite))
			{
				PackageRelationship docPackageRelationship = wdPackage.GetRelationshipsByType(customUiRelationshipType).FirstOrDefault();
				Uri documentUri = null;
				if (docPackageRelationship == null)
				{
					documentUri = PackUriHelper.CreatePartUri(new Uri(customUiXmlUri, UriKind.Relative));
					wdPackage.CreatePart(documentUri, "application/xml");
					docPackageRelationship = wdPackage.CreateRelationship(documentUri, TargetMode.Internal, customUiRelationshipType, CustomUiId);
				}

				if (docPackageRelationship != null)
				{
					if (documentUri == null)
						documentUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), docPackageRelationship.TargetUri);

					PackagePart documentPart = wdPackage.GetPart(documentUri);
					string customXml = "<customUI xmlns=\"http://schemas.microsoft.com/office/2006/01/customui\"><ribbon><tabs><tab id=\"tbKarta\" label=\"Karta\"></tab></tabs></ribbon></customUI>";
					if (customUiXml != null)
						using (StreamReader streamReader = new StreamReader(customUiXml))
						{
							customXml = streamReader.ReadToEnd();
						}

					using (StreamWriter streamWriter = new StreamWriter(documentPart.GetStream(FileMode.Create)))
					{
						streamWriter.Write(customXml);
						uspech = true;
					}
				}
			}

			return uspech;
		}

		private static void ZalohovatDokument(string souborDocx, bool zalohovat)
		{
			if (zalohovat)
			{
				string zaloha = DejNazevZalohy(souborDocx);
				File.Copy(souborDocx, zaloha);
			}
		}

		private static string DejNazevZalohy(string souborDocx)
		{
			string zaloha = souborDocx + ".bak";
			while (File.Exists(zaloha))
			{
				zaloha = souborDocx + string.Format("_{0:yyyy-MM-dd-hh-mm-ss}{1}", DateTime.Now, ".bak");
			}
			return zaloha;
		}

		public static bool VytvoritCustomUI(string souborDocx)
		{
			return VytvoritCustomUI(souborDocx, null);
		}

		[Obsolete("Použijte metodu VytvoritCustomUI s parametrem customUiXml", false)]
		public static bool NahraditCustomUI(string strDocx, string customUiXml)
		{
			return VytvoritCustomUI(strDocx, customUiXml);
		}

		public static void ExtrahovatDoSouboru(string strDocx, string strVystup, bool blnFormatovatOdsazeni)
		{

			using (FileStream fs = new FileStream(strDocx, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (StreamReader sr = new StreamReader(fs))
				{
					using (Package wdPackage = Package.Open(sr.BaseStream))
					{


						PackagePart documentPart = null;

						foreach (PackageRelationship relationship in wdPackage.GetRelationshipsByType(RelDocumentRelationshipType))
						{
							Uri documentUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), relationship.TargetUri);
							documentPart = wdPackage.GetPart(documentUri);
							break; //only one part
						}

						UlozitPackagePart(strVystup, blnFormatovatOdsazeni, documentPart);
						wdPackage.Close();
					}
				}
			}
		}

		public static void ExtrahovatRelaceDokumentuDoSouboru(string strDocx, string strVystup, bool blnFormatovatOdsazeni)
		{
			using (FileStream fs = new FileStream(strDocx, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (StreamReader sr = new StreamReader(fs))
				{
					using (Package wdPackage = Package.Open(sr.BaseStream))
					{
						PackagePart documentPart = null;
						PackagePart relationDocumentPart = null;


						foreach (PackageRelationship relationship in wdPackage.GetRelationshipsByType(RelDocumentRelationshipType))
						{
							Uri documentUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), relationship.TargetUri);

							//documentPart = pack.GetPart(documentUri);

							string relativeUri = documentUri.OriginalString.Substring(0, documentUri.OriginalString.LastIndexOf('/') + 1) +
							                     "_rels" + documentUri.OriginalString.Substring(documentUri.OriginalString.LastIndexOf('/')) +
							                     ".rels";

							Uri relationDocumentUri = new Uri(relativeUri, UriKind.Relative);

							if (wdPackage.PartExists(relationDocumentUri))
							{
								relationDocumentPart = wdPackage.GetPart(relationDocumentUri);
								UlozitPackagePart(strVystup, blnFormatovatOdsazeni, relationDocumentPart);
							}
							/*
				foreach (PackageRelationship documentRelation in documentPart.GetRelationshipsByType(RelDocumentRelationshipsType))
				{
					

					relationDocumentPart = pack.GetPart(relationDocumentUri);
					UlozitPackagePart(strVystup, blnFormatovatOdsazeni, relationDocumentPart);
				}
		*/
							break; //only one part
						}


						wdPackage.Close();
					}
				}
			}
		}

		public static bool ExtrahovatPoznamkyPodCarouDoSouboru(string strDocx, string strVystup, bool blnFormatovatOdsazeni)
		{

			using (FileStream fs = new FileStream(strDocx, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (StreamReader sr = new StreamReader(fs))
				{
					using (Package wdPackage = Package.Open(sr.BaseStream))
					{
						PackagePart documentPart = null;
						PackagePart footnotePart = null;
						Uri documentUri = null;

						foreach (PackageRelationship relationship in wdPackage.GetRelationshipsByType(RelDocumentRelationshipType))
						{
							documentUri = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), relationship.TargetUri);
							documentPart = wdPackage.GetPart(documentUri);
							break; //only one part
						}

						if (documentPart != null)
						{
							foreach (PackageRelationship relationship in documentPart.GetRelationshipsByType(RelFootnotesRelationshipType))
							{
								Uri footnoteUri = PackUriHelper.ResolvePartUri(documentUri, relationship.TargetUri);
								footnotePart = wdPackage.GetPart(footnoteUri);
								break; //only one part
							}
						}

						if (footnotePart == null)
						{
							wdPackage.Close();
							return false;
						}
						UlozitPackagePart(strVystup, blnFormatovatOdsazeni, footnotePart);
						wdPackage.Close();
					}
				}
			}
			return true;

		}

		private static void UlozitPackagePart(string strVystup, bool blnFormatovatOdsazeni, PackagePart documentPart)
		{
			if (documentPart == null)
				return;
			if (blnFormatovatOdsazeni)
			{
				try
				{
					XmlReaderSettings xrs = new XmlReaderSettings();
					xrs.CloseInput = false;
					xrs.ConformanceLevel = ConformanceLevel.Document;
					xrs.IgnoreWhitespace = true;
					//xr = XmlReader.Create(documentPart.GetStream(), xrs);
					using (XmlReader xr = XmlReader.Create(documentPart.GetStream(), xrs))
					{

						XmlWriterSettings xws = new XmlWriterSettings();
						xws.CloseOutput = true;
						xws.ConformanceLevel = ConformanceLevel.Document;
						xws.Indent = true;
						xws.IndentChars = " ";
						using (XmlWriter xw = XmlWriter.Create(strVystup, xws))
						{
							xw.WriteNode(xr, false);
							//while (xr.Read()) {
							//  xw.WriteNode(xr, false);
							//  if (xr.IsEmptyElement && xr.Depth == 0)
							//    xw.WriteNode(xr, false);
							//}
						}
					}

				}
				catch (Exception e)
				{

					throw e;
				}

			}
			else
			{

				StreamReader sr = new StreamReader(documentPart.GetStream());
				StreamWriter sw = new StreamWriter(strVystup, false, sr.CurrentEncoding);
				while (!sr.EndOfStream)
					sw.WriteLine(sr.ReadLine());
				sr.Close();
				sw.Close();
			}
		}


		private static string GetTempFileName()
		{
			return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp"); 
		}

		public static bool VycistiZnackovaniDokumentu(string souborDocx, bool zalohovat, string novySoubor)
		{
			
			string dokument = GetTempFileName();

			ZalohovatDokument(souborDocx, zalohovat);

			ExtrahovatDoSouboru(souborDocx, dokument, true);

			DocxCleaner cleaner = new DocxCleaner();

			CleaningResult result = cleaner.Clean(dokument);



			if(result.Success)
			{
					string zaloha = GetTempFileName();
				if(souborDocx != novySoubor)
				{
					File.Delete(zaloha);
					File.Copy(souborDocx, zaloha);
				}
				NahraditDokument(souborDocx, result.Output, zalohovat);
				if(souborDocx != novySoubor)
				{
					if(File.Exists(novySoubor))
						File.Delete(novySoubor);
					File.Move(souborDocx, novySoubor);
					File.Move(zaloha, souborDocx);
				}
			}

			return result.Success;
		}

		/// <summary>
		/// Vyčistí značky v dokumentu DOCX. Sloučí sousední prvky se stejným formátováním.
		/// </summary>
		/// <param name="souborDocx">Soubor DOCX, jehož značkování se má vyčistit.</param>
		/// <param name="zalohovat">Zda se má soubor DOCX před vyčištěním zálohovat.</param>
		public static bool VycistiZnackovaniDokumentu(string souborDocx, bool zalohovat)
		{
			return VycistiZnackovaniDokumentu(souborDocx, zalohovat, souborDocx);
		}

	}
}
