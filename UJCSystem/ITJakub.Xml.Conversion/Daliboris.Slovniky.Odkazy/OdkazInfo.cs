using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Daliboris.Pomucky.Databaze.Zaznamy;

namespace Daliboris.Slovniky {

	[DebuggerDisplay("{Zdroj}, {Identifikator}, {OdkazovyText} ({Heslo}), {HesloveSlovo}, [z {Cil}]")]
	public class OdkazInfo : IComparable<OdkazInfo>, IZaznam<OdkazInfo> {
		private const char cchDelimitator = Daliboris.Pomucky.Texty.Znaky.OddelovacPoli;

		/// <summary>
		/// Výchozí heslové slovo heslové stati, v němž se odkaz objevuje
		/// </summary>
		public string HesloveSlovo { get; set; }
		/// <summary>
		/// Text odkazu (co se objevuje v textu), tj. včetně morfologické charakteristiky, závorek, čísla významu ap.
		/// </summary>
		public string OdkazovyText { get; set; }
		/// <summary>
		/// Heslové slovo, na nějž se odkazuje (bez podrobnějších údajů o homonymu, čísle významu ap.)
		/// </summary>
		public string Heslo { get; set; }

		/// <summary>
		/// Číslo homonyma
		/// </summary>
		public string Homonymum { get; set; }


		/// <summary>
		/// Prefix, text před heslovým slovem, obvykle hvězdička nebo křížek.
		/// </summary>
		public string Prefix { get; set; }


		/// <summary>
		/// Jednotná reprezentace čísla homonyma pomocí orních indexů.
		/// </summary>
		/// <remarks>Uplatní se při porovnávání s hesly různých slovníků, kde se používají jiné značky pro odlišení homonym.</remarks>
		public string HomonymumJednotne {
			get {
				switch (Homonymum) {
					case "1":
						return "¹";
					case "2":
						return "²";
					case "3":
						return "²";
					default:
						return Homonymum;
				}
			}
		}
		/// <summary>
		/// Zdroj, v němž se text nachází
		/// </summary>
		public string Zdroj { get; set; }
		/// <summary>
		/// Cíl, v němž se heslové slovo nachází
		/// </summary>
		public string Cil { get; set; }
		/// <summary>
		/// Identifikátor heslového slova v cíli
		/// </summary>
		public string Identifikator { get; set; }


		public OdkazInfo() { }

		/// <summary>
		/// Rozdělí vstupní text na jednotlivé položky odkazového hesla. Odstraní redundantní text.
		/// </summary>
		/// <param name="strText">Odkazujícíc text, jak se vyskytuje ve slovníku.</param>
		/// <param name="strCil">Cílový zdroj, na něj odkaz dkazuje.</param>
		/// <returns>Vrací odkaz zpracovaný formátu OdkazInfo.</returns>
		public static OdkazInfo ZpracujOdkaz(string  strText, string  strCil)
		{
			OdkazInfo oi = new OdkazInfo();

			oi.OdkazovyText = strText;
			oi.Cil = strCil;

			//string[] asText = strText.Split(new char[] {' '});
			//TODO Vyřešit případ typu "prázdný B2"

			StringBuilder sbHeslo = new StringBuilder(strText.Trim()); //nenašla se čísla homonyma, pokud text končil mezerou
			
			string[] asKonce = new string[] { " jen ipf.", " jen pf.", " adj.", " pf.", " adv.", " ipf.", " f.", " n.", " m.", " pron.", " I", " interj." };
			//TODO Využít rozdělení pomocí regulárních výrazů
			foreach (string item in asKonce) {
				int i = strText.IndexOf(item, StringComparison.CurrentCulture);
				if (i == -1) continue;
				sbHeslo.Remove(i, sbHeslo.Length - i);
				break;
			}

			int iKonec = sbHeslo.ToString().IndexOfAny(",(1234567890“".ToCharArray());
			if (iKonec > -1)
				sbHeslo.Remove(iKonec, sbHeslo.Length - iKonec);
			
			//TODO Neměl by se oddělit i křížek, popř. pomlčka (ta by se ale v odkaze vyskytovat neměla - i když možná ano)?
			if(sbHeslo[0] == '*' || sbHeslo[0]  == 'ˣ')
			{
				oi.Prefix = sbHeslo[0].ToString();
				sbHeslo.Remove(0, 1);
			}
			if ("¹²³".IndexOf(sbHeslo[sbHeslo.Length - 1]) > -1)
			{
				oi.Homonymum = sbHeslo[sbHeslo.Length - 1].ToString();
				sbHeslo.Remove(sbHeslo.Length - 1, 1);
			}

			//odstranit uvozovky u staročeských výrazů, typ „přěľútný“
			if (sbHeslo[0] == '„')
			{
				sbHeslo.Remove(0, 1);
			}
			if (sbHeslo[sbHeslo.Length - 1] == '“')
			{
			 sbHeslo.Remove(sbHeslo.Length - 1, 1);
			}

			oi.Heslo = sbHeslo.ToString().Trim();

			return oi;
		}

		private void NacistZaznam(string strZaznam) {
			string[] asZaznam = strZaznam.Split(new char[] { cchDelimitator });
			Prefix = PriradNullNeboText(asZaznam[0]);
			Heslo = PriradNullNeboText(asZaznam[1]);
			Homonymum = PriradNullNeboText(asZaznam[2]);
			Zdroj = PriradNullNeboText(asZaznam[3]);
			Cil = PriradNullNeboText(asZaznam[4]);
			Identifikator = PriradNullNeboText(asZaznam[5]);
			OdkazovyText = PriradNullNeboText(asZaznam[6]);
			HesloveSlovo = PriradNullNeboText(asZaznam[7]);

		}

		public OdkazInfo(string strZaznam) {
			NacistZaznam(strZaznam);
		}
		public OdkazInfo(string  strPrefix, string strHeslo, string strHomonymum, string strZdroj, string strCil, string strIdentifikator, string strHesloveSlovo, string strOdkazovyText) {
			Prefix        = strPrefix;
			Heslo         = strHeslo;
			Homonymum     = strHomonymum;
			Zdroj         = strZdroj;
			Cil           = strCil;
			Identifikator = strIdentifikator;
			OdkazovyText  = strOdkazovyText;
			HesloveSlovo  = strHesloveSlovo;

			if (strHeslo == null)
				Heslo = OdkazovyText;
		}

		public string Zaznam() {
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0}{1}", Prefix, cchDelimitator);
			sb.AppendFormat("{0}{1}", Heslo, cchDelimitator);
			sb.AppendFormat("{0}{1}", Homonymum, cchDelimitator);
			sb.AppendFormat("{0}{1}", Zdroj, cchDelimitator);
			sb.AppendFormat("{0}{1}", Cil, cchDelimitator);
			sb.AppendFormat("{0}{1}", Identifikator, cchDelimitator);
			sb.AppendFormat("{0}{1}", OdkazovyText, cchDelimitator);
			sb.AppendFormat("{0}"   , HesloveSlovo);
			return sb.ToString();
		}

		private static string PriradNullNeboText(string sText) {
			return String.IsNullOrEmpty(sText) ? null : sText;
		}

		#region IComparable<OdkazInfo> - členové

		public int CompareTo(OdkazInfo other) {
			int i = 1;
			if (other == null)
				return 1;
			i = String.Compare(this.Heslo, other.Heslo, false);
			if (i != 0)
				return i;
			i = String.Compare(this.HomonymumJednotne, other.HomonymumJednotne, false);
			if (i != 0)
				return i;
			i = String.Compare(this.Cil, other.Cil);
			if (i != 0)
				return i;
			i = String.Compare(this.Identifikator, other.Identifikator);
			return i;
		}

		#endregion



		#region IZaznam<OdkazInfo> Members

		public void NactiZaznam(string strZaznam) {
			NacistZaznam(strZaznam);
		}

		public void NactiZaznam(string strZaznam, char chOddelovacPoli) {
			//TODO
		}

		public void NactiZaznam(string strZaznam, char chOddelovacPoli, char chOddelovacHodnot) {
			//TODO
		}

		public string VytvorZaznam() {
			return Zaznam();
		}

		public string VytvorZaznam(char chOddelovacPoli) {
			return Zaznam();
			//TODO
		}

		public string VytvorZaznam(char chOddelovacPoli, char chOddelovacHodnot) {
			return Zaznam();
			//TODO
		}

		private char mchOddelovacPoli;
		public char OddelovacPoli {
			get {
				return mchOddelovacPoli;
				//TODO
			}
			set {
				mchOddelovacPoli = value;
				//TODO
			}
		}

		private char mchOddelovacHodnot;
		public char OddelovacHodnot {
			get {
				return mchOddelovacHodnot;
				//TODO
			}
			set {
				mchOddelovacHodnot = value;
				//TODO
			}
		}

		#endregion
	}
}
