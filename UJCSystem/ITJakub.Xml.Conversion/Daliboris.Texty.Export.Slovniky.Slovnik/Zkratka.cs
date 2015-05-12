using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Daliboris.Slovniky {
	public class Zkratka {

		bool mblnObsahujeTecku;
		bool mblnZacinaMalymPismenem;

		public Zkratka(string strZdroj, string strTyp, string strText, string strId, string strRozepsani) {
			Zdroj = strZdroj;
			Typ = strTyp;
			Text = strText;
			Id = strId;
			Rozepsani = strRozepsani;
		}

		public Zkratka(string strZdroj, string strTyp, string strText, string strId, string strRozepsani, bool blnDoplneno) : this(strZdroj, strTyp, strText,strId, strRozepsani) {
			Doplneno = blnDoplneno;
		}

		private string mstrText;
		private int mintDelka;
		public string Text {
			get { return mstrText; }
			set {
				mstrText = value;
				if (mstrText == null) {
					mintDelka = -1;
					mblnZacinaMalymPismenem = mblnObsahujeTecku = false;
					}
				else {
					mintDelka = mstrText.Length;
					mblnZacinaMalymPismenem = Char.IsLower(mstrText, 0);
					mblnObsahujeTecku = (mstrText.IndexOf('.') > -1);
				}
			}
		}
		public int Delka { get { return mintDelka; } }
		public bool ObsahujeTecku { get { return mblnObsahujeTecku; } }
		public bool ZacinaMalymPismenem { get { return mblnZacinaMalymPismenem; } }

		public string Id { get; set; }
		public string Typ { get; set; }
		public string Zdroj { get; set; }
		public string Rozepsani { get; set; }
		public bool Doplneno { get; set; }

	}


	public class Zkratky : ICollection<Zkratka> {

		private Dictionary<string, Zkratka> mgdcZkratky = new Dictionary<string, Zkratka>();

		public bool ObsahujeZkratu(string strText) {
			return mgdcZkratky.ContainsKey(strText);
		}

		public Zkratka this[string strText] {
			get { return mgdcZkratky[strText]; }
			set { mgdcZkratky[strText] = value; }
		}

		public void Append(Zkratka item) {
			if (ObsahujeZkratu(item.Text)) {
				if (mgdcZkratky[item.Text].Rozepsani == item.Rozepsani) {
					if ((mgdcZkratky[item.Text].Typ == "pam" && item.Typ == "pram")) {
						mgdcZkratky[item.Text] = item;
					}
					else if (!(mgdcZkratky[item.Text].Typ == "pram" && item.Typ == "pam")) {
						mgdcZkratky[item.Text].Id += "|" + item.Id;
						mgdcZkratky[item.Text].Typ += "|" + item.Typ;
						mgdcZkratky[item.Text].Rozepsani += "|" + item.Rozepsani;
					}
				}
				else if (mgdcZkratky[item.Text].Typ == item.Typ) {
					mgdcZkratky[item.Text].Id += "|" + item.Id;
					mgdcZkratky[item.Text].Rozepsani += "|" + item.Rozepsani;
				}
				else {
					mgdcZkratky[item.Text].Id += "|" + item.Id;
					mgdcZkratky[item.Text].Rozepsani += "|" + item.Rozepsani;
					mgdcZkratky[item.Text].Typ += "|" + item.Typ;
				}
			}
			else
				mgdcZkratky.Add(item.Text, item);
			if (Char.IsLower(item.Text, 0)) {
				string sText = Char.ToUpper(item.Text[0]).ToString() + item.Text.Substring(1);
				Zkratka zkV = new Zkratka(item.Zdroj, item.Typ, sText, item.Id, item.Rozepsani);
				this.Append(zkV);
			}
		}

		#region ICollection<Zkratka> Members


		public void Add(Zkratka item) {
			if (ObsahujeZkratu(item.Text)) {
				if (mgdcZkratky[item.Text].Rozepsani == item.Rozepsani) {
					if ((mgdcZkratky[item.Text].Typ == "pam" && item.Typ == "pram")) {
						mgdcZkratky[item.Text] = item;
					}
					else if (!(mgdcZkratky[item.Text].Typ == "pram" && item.Typ == "pam")) {
						mgdcZkratky[item.Text].Id += "|" + item.Id;
						mgdcZkratky[item.Text].Typ += "|" + item.Typ;
						mgdcZkratky[item.Text].Rozepsani += "|" + item.Rozepsani;
					}
				}
				else if (mgdcZkratky[item.Text].Typ == item.Typ) {
					mgdcZkratky[item.Text].Id += "|" + item.Id;
					mgdcZkratky[item.Text].Rozepsani += "|" + item.Rozepsani;
				}
				else {
					mgdcZkratky[item.Text].Id += "|" + item.Id;
					mgdcZkratky[item.Text].Rozepsani += "|" + item.Rozepsani;
					mgdcZkratky[item.Text].Typ += "|" + item.Typ;
				}
			}
			else
				mgdcZkratky.Add(item.Text, item);
			//if (Char.IsLower(item.Text, 0)) {
			//  string sText = Char.ToUpper(item.Text[0]).ToString() + item.Text.Substring(1);
			//  Zkratka zkV = new Zkratka(item.Zdroj, item.Typ, sText, item.Id, item.Rozepsani);
			//  this.Append(zkV);
			//}
		}

		public void Clear() {
			mgdcZkratky.Clear();
		}

		public bool Contains(Zkratka item) {
			return ObsahujeZkratu(item.Text);
		}

		public void CopyTo(Zkratka[] array, int arrayIndex) {
			mgdcZkratky.Values.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return mgdcZkratky.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool Remove(Zkratka item) {
			return mgdcZkratky.Remove(item.Text);
		}

		#endregion

		#region IEnumerable<Zkratka> Members

		public IEnumerator<Zkratka> GetEnumerator() {
			return mgdcZkratky.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		#endregion

		public static Zkratky NactiZkratky(string strSoubor) {
			Zkratky zkr = new Zkratky();
			using (System.IO.StreamReader sr = new StreamReader(strSoubor)) {
				string strRadek = null;
				while ((strRadek = sr.ReadLine()) != null)
				{
                    //odstranìní otazníkù, které zùstaly ve zdrojových datech
				    if (strRadek.Contains(" ??"))
				        strRadek = strRadek.Replace(" ??", "");
                    if (strRadek.Contains("??"))
                        strRadek = strRadek.Replace("??", "");

					string[] asR = strRadek.Split('|');
					if (asR.Length == 6)
						zkr.Append(new Zkratka(asR[0], asR[1], asR[3], asR[2], asR[5]));
					else if(asR.Length == 7)
					 if(asR[5].Length == 0)
						zkr.Append(new Zkratka(asR[0], asR[1], asR[3], asR[2], asR[3]));
				 else
						zkr.Append(new Zkratka(asR[0], asR[1], asR[3], asR[2], asR[5]));
					else
						zkr.Append(new Zkratka(asR[0], asR[1], asR[3], asR[2], asR[3]));
				}

			}
			return zkr;
		}

		internal Zkratka NajdiZkratku(string strText) {
			return NajdiZkratku(strText, null);
		}

		internal Zkratka NajdiZkratku(string strText, string strType) {
			char[] ch = new char[] { ')', ']', '!' };
			int iDelka = strText.TrimEnd(ch).Length;
			var zkDlouhe = this.Where(c => c.Delka == iDelka);
			if (strType != null) {
				if (strType == "bible") {
					zkDlouhe = this.Where(c => c.Typ == strType);
				}
				else
					zkDlouhe = zkDlouhe.Where(c => c.Typ == strType);
			}
			foreach (Zkratka zkr in zkDlouhe) {
				if (strType == "bible") {
					if (strText.StartsWith(zkr.Text, false, null)) {
						return zkr;
					}
				}
				else {
					if (iDelka == strText.Length) {
						if (String.Compare(zkr.Text, strText, false) == 0) {
							return zkr;
						}
					}
					else {
						if (strText.StartsWith(zkr.Text, false, null)) {
							return zkr;
						}
					}
				}
			}
			zkDlouhe = this.Where(c => c.Delka == iDelka);
			foreach (Zkratka zkr in zkDlouhe) {
				if (String.Compare(zkr.Text, strText, false) == 0) {
					if (strType != null && zkr.Typ.Contains(strType)) {
						char[] chSv = new char[] { '|' };
						string[] asTyp = zkr.Typ.Split(chSv);
						for (int i = 0; i < asTyp.Length; i++) {
							if (asTyp[i] == strType) {
								string[] asPole = zkr.Id.Split(chSv);
								string sId = asPole[i];
								asPole = zkr.Rozepsani.Split(chSv);
								string sRozepsani = asPole[i];
								Zkratka zk = new Zkratka(zkr.Zdroj, strType, zkr.Text, sId, sRozepsani);
								return zk;
							}
						}
						return zkr;
					}

				}
			}
			return null;
		}

		internal Zkratky NajdiZkratky(string strText, string strType) {
			// if (strText.Contains(" ")) {
			Zkratky zkrt = new Zkratky();
			string[] astrText = strText.Split(new char[] { ' ' });
			for (int i = 0; i < astrText.Length; i++) {
				Zkratka z = NajdiZkratku(astrText[i], strType);
				if (z != null)
					zkrt.Add(z);
			}
			if (zkrt.Count == 0) {
				return null;
			}
			else
				return zkrt;
			//}
			return null;
		}

	}

}