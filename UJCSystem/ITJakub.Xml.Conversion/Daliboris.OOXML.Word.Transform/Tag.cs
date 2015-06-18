using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Daliboris.OOXML.Word.Transform {
	public class Tag {

		public const string csDelimitator = "^";
		public static string GetIdentifikator(string strStyl, string strPredchoziStyl) {
			if (String.IsNullOrEmpty(strPredchoziStyl))
				return strStyl;
			else {
				return strStyl + csDelimitator + strPredchoziStyl;
			}
		}
		public Tag() { }
		[Obsolete("Starší kód pro transofrmaci z Xml")]
		public Tag(bool blnBezZnacky, bool blnIgnorovat, string strNamespace,
			string strNazev, bool blnPrazdnyElement, string strPredchoziStyl,
			bool blnSloucitSPredchozim, string strStyl, Atributy atAtributy,
			Nahrady nhNahrady) {

			this.BezZnacky = blnBezZnacky;
			this.Ignorovat = blnIgnorovat;
			this.Namespace = strNamespace;
			this.Nazev = strNazev;
			this.PrazdnyElement = blnPrazdnyElement;
			this.PredchoziStyl = strPredchoziStyl;
			this.SloucitSPredchozim = blnSloucitSPredchozim;
			this.Styl = strStyl;
			this.Atributy = atAtributy;
			this.Nahrady = nhNahrady;
		}

		public Tag(bool blnBezZnacky, bool blnIgnorovat, string strNamespace,
			string strNazev, bool blnPrazdnyElement, string strPredchoziStyl,
			bool blnSloucitSPredchozim, string strNasledujiciStyl, bool blnSloucitSNasledujcim,
			string strStyl, Atributy atAtributy, Nahrady nhNahrady)
			: this(blnBezZnacky, blnIgnorovat, strNamespace,
	strNazev, blnPrazdnyElement, strPredchoziStyl,
	blnSloucitSPredchozim, strStyl, atAtributy,
	nhNahrady) {
			this.SloucitSNasledujicim = blnSloucitSNasledujcim;
			this.NasledujiciStyl = strNasledujiciStyl;
		}

		public Atributy Atributy { get; set; }
		public Nahrady Nahrady { get; set; }
		public bool BezZnacky { get; set; }
		public bool Ignorovat { get; set; }
		public string Nazev { get; set; }
		public string Namespace { get; set; }
		public bool PrazdnyElement { get; set; }
		public string PredchoziStyl { get; set; }
		public bool SloucitSPredchozim { get; set; }
		public string Styl { get; set; }
		public string NasledujiciStyl { get; set; }
		public bool SloucitSNasledujicim { get; set; }
		public TypTagu TypTagu { get; set; }

		public string Identifikator {
			get { return Tag.GetIdentifikator(this.Styl, this.PredchoziStyl); }
		}

	}
}
