using System;
using System.Text;
using Daliboris.Pomucky.Texty;
using System.Diagnostics;

namespace Daliboris.Slovniky {

	public  static  class  TextoveKonstanty
	{
		public const char HorniIndex1 = '¹';
		public const char KrizekStcS = 'ˣ';
	}

	/// <summary>
	/// Podrobné informace o heslovém slově ze slovníku.
	/// </summary>

	[DebuggerDisplay("{Prefix}, {HesloveSlovo}, {Postfix}, {Id}, {ZkratkaZdroje}, {HeslovaStatId}, {JeRef}")]
	public class HesloInfo : IComparable<HesloInfo> {
		private string mstrForm;
		private int mintFormId;
		private int mintHeslovaStatTypId;
		private string mstrHeslovaStatTyp;
		private int mintZdrojId;
		private string mstrZkratkaZdroje;
		private int mintZpusobVyuzitiId;
		private string mstrZpusobVyuziti;
		private const char cchDelimitator = Znaky.OddelovacPoli;


		/// <summary>
		/// Prefix heslového slova - značka uváděná na jeho začátku.
		/// Obvykle jde o hvězdičku nebo křížek
		/// </summary>
		public string Prefix { get; set; } //0
		/// <summary>
		/// Heslové slovo bez nadbytečných údajů na začátku nebo na konci hesla
		/// </summary>
		public string HesloveSlovo { get; set; } //1
		/// <summary>
		/// Doplňující údaj za heslovým slovem, např. (?)
		/// </summary>
		public string Postfix { get; set; } //2
		/// <summary>
		/// Číslo homonyma; může jít o číslici v horním indexu, nebo normální číslici
		/// </summary>
		public string Homonymum { get; set; } //3

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
		/// Zkratka slovního druhu, uváděná u homony (platí pro HesStčS)
		/// </summary>
		public string SlovniDruh { get; set; } //4
		/// <summary>
		/// Další informace uváděné u heslového slova (platí pro HesStčS)
		/// </summary>
		public string DalsiInformace { get; set; } //5

		/// <summary>
		/// Variantní podoba heslového slova (platí pro HesStčS)
		/// </summary>
		public string Varianta { get; set; } //6

		/// <summary>
		/// Identifikátor heslového slova
		/// </summary>
		public string Id { get; set; } //7 

		/// <summary>
		/// Identifikátor pro formu heslového slova. Údaj převzatý z databáze.
		/// </summary>
		public int FormaId {
			get { return mintFormId; }
			set { mintFormId = value; }
		}  //8

		/// <summary>
		/// Forma heslového slova. Údaj převzatý z databáze.
		/// Možné hodnoty: norm (plné heslo); restored (rozepsané heslo); short (zkrácené heslo)
		/// Výchozí hodnota je norm.
		/// </summary>
		public string Forma {
			get {
				return (mstrForm);
			}
			set {
				mstrForm = value;
				if (mstrForm == null || !Heslar.TypHeslovehoSlova.ContainsKey(mstrForm))
					mstrForm = "norm";
				mintFormId = Heslar.TypHeslovehoSlova[mstrForm];
			}
		} //8

		private string mstrTypHeslovehoSlova;
		/// <summary>
		/// Typ heslového slova. Používá se v ESSČ.
		/// Možné hodnoty: substandard; null.
		/// V MSS může mít hodnotu "subentry".
		/// </summary>
		public string TypHeslovehoSlova {
			get {
				return mstrTypHeslovehoSlova;
			}
			set {
			  if (value == null)
				mstrTypHeslovehoSlova = "0";
			  else if (value == "4")
			  	mstrTypHeslovehoSlova = value;
			  else
				  mstrTypHeslovehoSlova = "1";
					//mstrType = value;
			}
		} //9

		public int PodhesloId {
			get { return (mstrTypHeslovehoSlova == "1") ? 1 : 0; }
		}

		private string mstrLang;
		/// <summary>
		/// Jazyk heslového slova. Uplatní se u vícejazyčných slovníků.
		/// </summary>
		public string Lang {
			get {
				return mstrLang;
			}
			set {
				if (value == null)
					mstrLang = "1";
				else
					mstrLang = value;
			}
		} //10

		/// <summary>
		/// Identifikátor heslové stati.
		/// </summary>
		public string HeslovaStatId { get; set; } //11

		/// <summary>
		/// K čmu slouží? V součastnosti se nepoužívá.
		/// </summary>
		public string HeslovaStat { get; set; } //12

		public int HeslovaStatTypId {
			get { return mintHeslovaStatTypId; }
			set { mintHeslovaStatTypId = value; }
		}

		/// <summary>
		/// Typ heslové stati.
		/// Možné hodnoty: norm (heslo v rámci hesláře); full (plnohodnotná heslová stať);
		/// ref (heslová stať obsahující pouze odkaz(y)); excl (vyloučené heslo ze slovníku (ESSČ))
		/// </summary>
		public string HeslovaStatTyp {
			get {
				return mstrHeslovaStatTyp;
			}
			set {
				mstrHeslovaStatTyp = value;
				if (value == null || !(Heslar.TypHesloveStati.ContainsKey(mstrHeslovaStatTyp)))
					mstrHeslovaStatTyp = "norm";
				if (JeRef == "true")
					mstrHeslovaStatTyp = "ref";
				mintHeslovaStatTypId = Heslar.TypHesloveStati[mstrHeslovaStatTyp];
			}
		} //13

		/// <summary>
		/// Identifikátor písmene (ve formátu lt00).
		/// </summary>
		public string Pismeno { get; set; } //14

		/// <summary>
		/// Identifiktátor zdroje (využívaný v databázích)
		/// </summary>
		public int ZdrojId {
			get { return mintZdrojId; }
		}

		/// <summary>
		/// Zkratka zdroje, v němž se heslové slovo nachází (HesStcS, ESSC, MSS atd.)
		/// </summary>
		public string ZkratkaZdroje {
			get {
				return mstrZkratkaZdroje;
			}
			set {
				mstrZkratkaZdroje = value;
				if(mstrZkratkaZdroje == null)
					throw new ArgumentNullException("Zkratka zdroje nebyla uvedena");
				if (!(Heslar.Zdroj.ContainsKey(mstrZkratkaZdroje)))
					throw new ArgumentOutOfRangeException("Zadaná zkratka není uvedena ve zdrojích: " + value);
				mintZdrojId = Heslar.Zdroj[mstrZkratkaZdroje];
			}
		} //15

		/// <summary>
		/// Retrogrdádní podoba heslového slova bez nenáležitých částí (postfixu a sufixu).
		/// </summary>
		public string Retrograd { get; set; } //16

		/// <summary>
		/// Zbylá část heslového slova, která netvoří součást retrográdu, např. zvratné sě, si.
		/// </summary>
		public string RetrogradZbytek { get; set; } //17


		/// <summary>
		/// Identifikátor způsobu využití heslové stati (1 = internal, 2 = public)
		/// </summary>
		public int ZpusobVyuzitiId {
			get {
				return mintZpusobVyuzitiId;
			}
		} //18

		/// <summary>
		/// Způsob použití heslové stati.
		/// Možné hodnoty: internal, public.
		/// </summary>
		public string ZpusobVyuziti {
			get {
				return mstrZpusobVyuziti;
			}
			set {

				mstrZpusobVyuziti = value ?? "public";
				if (!Heslar.ZpusobVyuziti.ContainsKey(mstrZpusobVyuziti))
					throw new ArgumentOutOfRangeException("Zadaný způsob využití není definován: " + mstrZpusobVyuziti);
				mintZpusobVyuzitiId = Heslar.ZpusobVyuziti[mstrZpusobVyuziti];
			}
		} //18

		/// <summary>
		/// Identifikátor heslového slova v HesStčS. Používá se u zpracování ESSČ.
		/// </summary>
		public string HesStcSIdRef { get; set; } //19

		public DateTime? PosledniZmena { get; set; }

		private string mstrJeRef;

		/// <summary>
		/// Zda je heslové slovo odkazové. Pokud ano, nastaví se <see cref="T:Daliboris.Slovniky.HesloInfo.TypHesloveStati" />
		/// 	na typ ref.
		/// Moné hodnoty: true, null.
		/// </summary>
		public string JeRef {
			get { return mstrJeRef; }
			set {
				mstrJeRef = value;
				if (mstrJeRef == "true")
					HeslovaStatTyp = "ref";
			}
		} //nepoužívá se ve výstupu

		/// <summary>
		/// Údaj, jestli je heslové slovo určeno pouze pro interní potřeby.
		/// </summary>
		/// <remarks>Interní je heslo tehdy, pokud se vyskytuje ve vyloučené heslové stati ESSČ, nebo pokud je způsob využití hesla nastaven na hodnotu internal</remarks>
		public  bool  JeInterni
		{
			get {
				if (ZpusobVyuzitiId == 2) return false;
				if(HeslovaStatTypId == 8) return true;
				if(ZpusobVyuzitiId == 1 ) return true;
				return false;
			}
		}

		/// <summary>
		/// Zpravuje heslové slovo, rozdělí jej na jednotlivé části (prefix a postfix) a vygeneruje retrográdní podobu.
		/// </summary>
		/// <param name="strText">Text reprezentující heslové slovo ve slovníku.</param>
		/// <param name="strPrefix">Prefix heslového slova uvedený v atributu prvku hw.</param>
		public bool ZpracujHesloveSlovo(string strText, string strPrefix) {

			if (String.IsNullOrEmpty(strText))
				return false;

			if (strPrefix == null) {
				if (!Char.IsLetter(strText, 0)) {
					for (int j = 1; j < strText.Length; j++) {
						if (Char.IsLetter(strText[j])) {
							strPrefix = strText.Substring(0, j);
							break;
						}
					}
				}
			}
			if (!String.IsNullOrEmpty(strPrefix)) {
				Prefix = strPrefix;
				HesloveSlovo = strText.Substring(strPrefix.Length);
			}
			else
				HesloveSlovo = strText;
			if (HesloveSlovo.Contains("(?)"))
				HesloveSlovo.Replace("(?)", "").TrimEnd();

			int i = HesloveSlovo.Length - 1;
			if (i == -1) {
				return false;
			}

			while (!Char.IsLetter(HesloveSlovo, i)) {
				if (HesloveSlovo[i] == ')' || HesloveSlovo[i] == '-')
					break;
				Postfix += HesloveSlovo[i].ToString();
				HesloveSlovo = HesloveSlovo.Substring(0, i);
				i--;
				if (i == -1) {
					break;
				}
			}
			if (i == -1) {
				return false;
			}
			string sRetrograd = Text.Retrograd(HesloveSlovo, true);

			if (sRetrograd.IndexOf(" ") > 0 && !(sRetrograd.Contains(" – ") || sRetrograd.Contains(" - "))) {
				Retrograd = sRetrograd.Substring(sRetrograd.LastIndexOf(" ") + 1);
				RetrogradZbytek = sRetrograd.Substring(0, sRetrograd.LastIndexOf(" "));
			}
			else
				Retrograd = sRetrograd;

			return true;

			#region Původní kód
			/*
			if (sRetrograd.IndexOf(" ") > 0 && !(sRetrograd.Contains(" – ") || sRetrograd.Contains(" - "))) {
				asVlastnosti[16] = sRetrograd.Substring(sRetrograd.LastIndexOf(" ") + 1);
				asVlastnosti[17] = sRetrograd.Substring(0, sRetrograd.LastIndexOf(" "));
			}
			else
				asVlastnosti[16] = sRetrograd;
			*/

			/*
													if (!String.IsNullOrEmpty(sPrefix)) {
										asVlastnosti[0] = sPrefix;
										asVlastnosti[1] = asVlastnosti[1].Substring(sPrefix.Length);
									}


									if (asVlastnosti[1].Contains("(?)")) {
										asVlastnosti[1] = asVlastnosti[1].Replace("(?)", "").TrimEnd();
									}
									int i = 0;
									//while (!Char.IsLetter(asVlastnosti[1], i))
									//{
									//   asVlastnosti[0] += asVlastnosti[1][i].ToString();
									//   asVlastnosti[1] = asVlastnosti[1].Substring(1);
									//   //i++;
									//}

									i = asVlastnosti[1].Length - 1;
									if (i == -1) {
										break;
									}
									while (!Char.IsLetter(asVlastnosti[1], i)) {
										if (asVlastnosti[1][i] == ')' || asVlastnosti[1][i] == '-')
											break;
										asVlastnosti[2] += asVlastnosti[1][i].ToString();
										asVlastnosti[1] = asVlastnosti[1].Substring(0, i);
										i--;
										if (i == -1) {
											break;
										}
									}
									if (i == -1) {
										break;
									}
									sRetrograd = Text.Retrograd(asVlastnosti[1], true);

									if (sRetrograd.IndexOf(" ") > 0 && !(sRetrograd.Contains(" – ") || sRetrograd.Contains(" - "))) {
										asVlastnosti[16] = sRetrograd.Substring(sRetrograd.LastIndexOf(" ") + 1);
										asVlastnosti[17] = sRetrograd.Substring(0, sRetrograd.LastIndexOf(" "));
									}
									else
										asVlastnosti[16] = sRetrograd;


				*/
			#endregion
		}


		public string Zaznam() {
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0}{1}", Prefix, cchDelimitator); //0
			sb.AppendFormat("{0}{1}", HesloveSlovo, cchDelimitator); //1 
			sb.AppendFormat("{0}{1}", Postfix, cchDelimitator); //2

			sb.AppendFormat("{0}{1}", Homonymum, cchDelimitator); //3
			sb.AppendFormat("{0}{1}", SlovniDruh, cchDelimitator); //4
			sb.AppendFormat("{0}{1}", DalsiInformace, cchDelimitator); //5
			sb.AppendFormat("{0}{1}", Varianta, cchDelimitator); //6

			sb.AppendFormat("{0}{1}", Id, cchDelimitator); //7
			sb.AppendFormat("{0}{1}", FormaId, cchDelimitator); //8
			sb.AppendFormat("{0}{1}", TypHeslovehoSlova, cchDelimitator); //nezobrazuje se //9 [ukládá se jako 0 nebo 1]
			sb.AppendFormat("{0}{1}", Lang, cchDelimitator); //nezobrazuje se //10
			sb.AppendFormat("{0}{1}", HeslovaStatId, cchDelimitator); //11
			sb.AppendFormat("{0}{1}", HeslovaStat, cchDelimitator); //12
			sb.AppendFormat("{0}{1}", HeslovaStatTypId, cchDelimitator); //13
			sb.AppendFormat("{0}{1}", Pismeno, cchDelimitator); //14
			sb.AppendFormat("{0}{1}", ZdrojId, cchDelimitator); //15
			sb.AppendFormat("{0}{1}", Retrograd, cchDelimitator); //16
			sb.AppendFormat("{0}{1}", RetrogradZbytek, cchDelimitator); //17
			/*
			if (ZpusobVyuzitiId == 0)
				sb.AppendFormat("{0}{1}", ""); //není-lí 1, nedávat nic; je vždy 0, není 1
			else
				sb.AppendFormat("{0}{1}", ZpusobVyuzitiId); //není-lí 1, nedávat nic; je vždy 0, není 1
			*/
			//18
			/*
			if (ZpusobVyuzitiId != 1)  //1 = internal; 2 = public
				sb.AppendFormat("{0}{1}", "", cchDelimitator); //není-lí 1, nedávat nic; je vždy 0, není 1
			else
			*/ 
			sb.AppendFormat("{0}{1}", ZpusobVyuzitiId, cchDelimitator); //18 //není-lí 1, nedávat nic; je vždy 0, není 1

			sb.AppendFormat("{0}{1}", HesStcSIdRef, cchDelimitator); //19
			sb.AppendFormat("{0}", PosledniZmena); //20 - pro HesStčS
		
			return sb.ToString();
		}


		public HesloInfo() { }

		/// <summary>
		/// Načte informace o hesle ze záznamu (hodnoty oddělené oddělovačem). <remarks>Jako oddělovač použije výchozí hodnotu oddělovače.</remarks>
		/// </summary>
		/// <param name="strZaznam">Text obsahující jednotlivé hodnoty oddělené oddělovačem.</param>
		public HesloInfo(string strZaznam) : this(strZaznam, cchDelimitator) { }

		/// <summary>
		/// Načte informace o hesle ze záznamu (hodnoty oddělené oddělovačem).
		/// </summary>
		/// <param name="strZaznam"></param>
		/// <param name="chDelimitator"></param>
		public HesloInfo(string strZaznam, char chDelimitator) {
			string[] asZaznam = strZaznam.Split(new char[] { chDelimitator });
			Prefix = PriradNullNeboText(asZaznam[0]);
			HesloveSlovo = PriradNullNeboText(asZaznam[1]);
			Postfix = PriradNullNeboText(asZaznam[2]);

			Homonymum = PriradNullNeboText(asZaznam[3]);
			SlovniDruh = PriradNullNeboText(asZaznam[4]);
			DalsiInformace = PriradNullNeboText(asZaznam[5]);
			Varianta = PriradNullNeboText(asZaznam[6]);

			Id = PriradNullNeboText(asZaznam[7]);
			FormaId = Int32.Parse(asZaznam[8]);
			TypHeslovehoSlova = PriradNullNeboText(asZaznam[9]);
			Lang = PriradNullNeboText(asZaznam[10]);

			HeslovaStatId = PriradNullNeboText(asZaznam[11]);
			HeslovaStat = PriradNullNeboText(asZaznam[12]);
			HeslovaStatTypId = Int32.Parse(asZaznam[13]);
			Pismeno = PriradNullNeboText(asZaznam[14]);
			mintZdrojId = Int32.Parse(asZaznam[15]);
			Retrograd = PriradNullNeboText(asZaznam[16]);
			RetrogradZbytek = PriradNullNeboText(asZaznam[17]);
			if (asZaznam[18].Length > 0)
				mintZpusobVyuzitiId = Int32.Parse(asZaznam[18]);
			//JeRef = PriradNullNeboText(asZaznam[19]);
			HesStcSIdRef = PriradNullNeboText(asZaznam[19]);
			if (asZaznam.Length == 21)
				PosledniZmena = PriradNullNeboDatum(asZaznam[20]);
		}

		private static string PriradNullNeboText(string sText) {
			return String.IsNullOrEmpty(sText) ? null : sText;
		}

	  private static DateTime? PriradNullNeboDatum(string text)
	  {
		if (String.IsNullOrEmpty(text))
			return null;
	  	DateTime dt;
		if(DateTime.TryParse(text, out dt))
			return dt;
	  	return null;
	  }

		#region IComparable<HesloInfo> - členové

		public int CompareTo(HesloInfo other) {
			int i = 1;
			if (other == null)
				return i;
			i = String.Compare(this.HesloveSlovo, other.HesloveSlovo, false);
			if (i != 0)
				return i;
			i = String.Compare(this.HomonymumJednotne, other.HomonymumJednotne);
			if (i != 0)
				return i;
			i = this.HeslovaStatTypId.CompareTo(other.HeslovaStatTypId);
			if (i != 0)
				return i;
			i = String.Compare(this.TypHeslovehoSlova, other.TypHeslovehoSlova);
			if (i != 0)
				return i;
			i = String.Compare(this.ZkratkaZdroje, other.ZkratkaZdroje);
			return i;
		}

		#endregion
	}
}
