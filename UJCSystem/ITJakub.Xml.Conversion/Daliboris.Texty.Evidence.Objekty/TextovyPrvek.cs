using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Daliboris.Texty.Evidence
{

	public class TextovePrvky : List<TextovyPrvek>
	{

		private string mstrText;
		#region Konstruktory
		public TextovePrvky() { }
		public TextovePrvky(string strText)
		{
			this.Text = strText;
		}

		public string Text
		{
			get { return mstrText; }
			set { mstrText = value;
			Analyzuj();
			}
		}

		#endregion

		public List<TextovyPrvek> PrvkyPodleTypu(TypTextovehoPrvku tpTyp)
		{
			List<TextovyPrvek> tps = new List<TextovyPrvek>(this.Count);
			foreach (TextovyPrvek tp in this)
			{
				if (tp.Typ == tpTyp)
				{
					tps.Add(tp);
				}
			}
			return tps;
		}

		public void Analyzuj()
		{
			if(mstrText == null)
				return;
			
			string[] asText;
			char[] chDelim = new char[] { ' ', '–', '/', ',' };
			char[] chVypust = new char[] {' ', ','};
			asText = RozdelitTextNaCasti(mstrText, chDelim, chVypust);


			for (int i = 0; i < asText.Length; i++) {
					TextovyPrvek tp = new TextovyPrvek(asText[i]);
					this.Add(tp);
			}
		}

		public void Analyzuj(string strText)
		{
			Text = strText;
		}

		/// <summary>
		/// Analyzuje jednotlivá slova v textu
		/// </summary>
		/// <param name="strText"></param>
		/// <returns></returns>
		public static List<TextovyPrvek> AnalyzujText(string strText) {
			TextovePrvky tps = new TextovePrvky();
			tps.Analyzuj(strText);
			return tps;
		}


		/// <summary>
		/// Rozdělí text na části, ktré oddělí pomocí předaného seznamu delimitátorů; 
		/// delimitátory zůstanou součástí vráceného pole prvků, pokud se neobjeví
		/// v seznamu delimitátorů, které se myají vypustit
		/// </summary>
		/// <param name="strText">Text, který se má rozdělit na jednotlivé části</param>
		/// <param name="chDelimitatory">Delimitátory, které v textu vymezují hranice jednotlivých částí</param>
		/// <param name="chDelimitatoryKVypusteni">Delimitátory, které se nedostanou do výsledku jako jednotlivá část</param>
		/// <returns>Vrací pole jednotlivých částí textu</returns>
		private static string[] RozdelitTextNaCasti(string strText, char[] chDelimitatory, 
			char[] chDelimitatoryKVypusteni) {
			List<string> gls = new List<string>();
			List<char> glsDelim = new List<char>(chDelimitatory);
			List<char> glsVypust = new List<char>();
			if(chDelimitatoryKVypusteni != null)
				glsVypust = new List<char>(chDelimitatoryKVypusteni);

			int iMax = strText.Length;
			List<char> glc = new List<char>(iMax);
			for (int i = 0; i < iMax; i++) {
				if (glsDelim.Contains(strText[i])) {
					if (glc.Count > 0)
						gls.Add(new string(glc.ToArray()));
					if (!glsVypust.Contains(strText[i]))
						gls.Add(strText[i].ToString());
					glc = new List<char>(iMax);

				}
				else
					glc.Add(strText[i]);
			}
			if (glc.Count > 0)
				gls.Add(new string(glc.ToArray()));

			return gls.ToArray();
		}

	}

	[DebuggerDisplay("{Text} = {Typ}")]
	public class TextovyPrvek {
		private static List<string> msglsKonektory = new List<string>();
		private static List<string> msglsUpresneni = new List<string>();
		private static List<string> msglsObdobi = new List<string>();
		private static List<string> msglsZlomek = new List<string>();

		private string mstrText;
		private TypTextovehoPrvku mttTyp;

		public string Text {
			get { return mstrText; }
			set {
				mstrText = value;
				AnalyzovatText();
			}
		}

		public TypTextovehoPrvku Typ {
			get { return mttTyp; }
		}





		private void AnalyzovatText() {
			string sText = mstrText.Trim();

			//jde o řadovou číslovku
			if (sText[sText.Length - 1] == '.') {
				sText = sText.Substring(0, sText.Length - 1);
			}
			int i;
			if (Int32.TryParse(sText, out i)) {
				switch (sText.Length.ToString()) {
					case "1":
						mttTyp = TypTextovehoPrvku.JednomistneCislo;
						break;
					case "2":
						mttTyp = TypTextovehoPrvku.DvojmistneCislo;
						break;
					case "4":
						mttTyp = TypTextovehoPrvku.CtyrmistneCislo;
						break;
					default:
						mttTyp = TypTextovehoPrvku.Neurceno;
						break;
				}

			}
			else {
				if (msglsKonektory.Contains(sText))
					mttTyp = TypTextovehoPrvku.Konektor;
				if (msglsObdobi.Contains(sText))
					mttTyp = TypTextovehoPrvku.Obdobi;
				if (msglsUpresneni.Contains(sText))
					mttTyp = TypTextovehoPrvku.Upresneni;
				if (msglsZlomek.Contains(sText))
					mttTyp = TypTextovehoPrvku.Zlomek;

			}

		}
		static TextovyPrvek() {
			msglsKonektory.Add("a");
			msglsKonektory.Add("nebo");

			//okolo, přelom, začátek, konec
			msglsUpresneni.Add("po");
			msglsUpresneni.Add("okolo");
			msglsUpresneni.Add("přelom");
			msglsUpresneni.Add("začátek");
			msglsUpresneni.Add("konec");
			msglsUpresneni.Add("post");
			msglsUpresneni.Add("ante");
			msglsUpresneni.Add("(?)");
			msglsUpresneni.Add("/");
			msglsUpresneni.Add("–");

			msglsObdobi.Add("léta");
			msglsObdobi.Add("století");
			msglsObdobi.Add("roku");
			msglsObdobi.Add("roce");

			msglsZlomek.Add("třetina");
			msglsZlomek.Add("čtvrtina");
			msglsZlomek.Add("polovina");

		}
		public TextovyPrvek() { }
		public TextovyPrvek(string strText) {
			Text = strText;
		}
	}
}