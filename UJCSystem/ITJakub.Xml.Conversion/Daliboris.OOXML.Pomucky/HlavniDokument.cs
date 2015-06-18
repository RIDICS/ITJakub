using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using Daliboris.OOXML.Word;

namespace Daliboris.OOXML.Pomucky {
 public class HlavniDokument {
	public const string csSubDocument = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/subDocument";
	public void Vytvor(string strHlavniSoubor, List<string> glsVnoreneSoubory) {
	 //Vytvořit hlavní dokument
	 using (WordprocessingDocument wordDocument =
		WordprocessingDocument.Create(strHlavniSoubor, WordprocessingDocumentType.Document)) {
		// Insert other code here. 

		//Pro všechny vnořené soubory:
		//vytvořit relaci
		//vložit odstavec a subDoc do hlavního dokumentu
		 FileInfo fi = new FileInfo(strHlavniSoubor);
		 string sSlozka = fi.DirectoryName;

		 MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

		 Document dc = new Document();
		 dc.AddNamespaceDeclaration("ve", "http://schemas.openxmlformats.org/markup-compatibility/2006");
		 dc.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
		 dc.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
		 dc.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
		 dc.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
		 dc.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
		 dc.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
		 dc.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
		 dc.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");


		 Body body = new Body();

		 foreach (string strSoubor in glsVnoreneSoubory) {
			Paragraph p;
			if(strSoubor.StartsWith(sSlozka))
			 p = VlozVnorenySoubor(mainPart, strSoubor.Substring(sSlozka.Length + 1), true);
			else
			 p = VlozVnorenySoubor(mainPart, strSoubor);
			body.Append(p);
		 }

		 dc.Append(body);
		 mainPart.Document = dc;
	 }

	 

	}
	private Paragraph VlozVnorenySoubor(MainDocumentPart mainPart, string strSoubor, bool blnRelativniCesta) {
	 ExternalRelationship er = null;
	 if(blnRelativniCesta)
		er = mainPart.AddExternalRelationship(csSubDocument, new System.Uri(strSoubor, System.UriKind.Relative));
	 else
		er = mainPart.AddExternalRelationship(csSubDocument, new System.Uri(strSoubor, System.UriKind.Absolute));

	 string sId = er.Id;

	 Paragraph p = new Paragraph();
	 ParagraphProperties pp = new ParagraphProperties();
	 
	 SectionProperties s = new SectionProperties();
	 pp.Append(s);

	 SubDocumentReference sdr = new SubDocumentReference() { Id = sId };

	 p.Append(pp);
	 p.Append(sdr);

	 return p;
	 
	}
	private Paragraph VlozVnorenySoubor(MainDocumentPart mainPart, string strSoubor) {
	 return VlozVnorenySoubor(mainPart, strSoubor, false);
	}
 }
}
