using System;
using System.Collections.Generic;
using System.Text;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence {
	public static class AnalyzatorDatace {
		private const int cintZacatekKonec = 15;
		private const int cintStoLet = 100;
		private const int cintPrelom = 10;
		private const int cintOkoloRoku = 3;
		private const int cintJenPolovina = 12;

		



		public const string csOtaznik = "(?)";
		public const string csOkoloRoku = "okolo roku";
		public const string csPoRoce = "po roce";
		public const string csPrelom = "přelom";
		public const string csJenPolovina = "polovina";
		public const string csPolovina = ". polovina";
		//		public const string csPolovina = "polovina";
		public const string cs1polovina = "1. polovina";
		public const string cs2polovina = "2. polovina";
		public const string csLeta = ". léta";
		public const string csStoleti = ". století";
		public const string csA = " a ";
		public const string csNebo = " nebo ";
		public const string csZacatek = "začátek";
		public const string csKonec = "konec";
		public const string csTretina = ". třetina";
		public const string csCtvrtina = ". čtvrtina";
		public const string csPost = "post";
		public const string csAnte = "ante";


		private static Dictionary<string, string> mgdcTexty;
/*
		const int cintPocetTextu = 14;
*/

		static AnalyzatorDatace() {
			mgdcTexty = new Dictionary<string, string>();
			//mgdcTexty.Add(cs1polovina, cs1polovina);
			//mgdcTexty.Add(cs2polovina, cs2polovina);
			mgdcTexty.Add(csPolovina, csPolovina);
			mgdcTexty.Add(csJenPolovina, csJenPolovina);

			mgdcTexty.Add(csA, csA);
			mgdcTexty.Add(csKonec, csKonec);
			mgdcTexty.Add(csLeta, csLeta);
			mgdcTexty.Add(csNebo, csNebo);
			mgdcTexty.Add(csOkoloRoku, csOkoloRoku);
			mgdcTexty.Add(csOtaznik, csOtaznik);
			//mgdcTexty.Add(csPolovina, csPolovina);
			mgdcTexty.Add(csPoRoce, csPoRoce);
			mgdcTexty.Add(csPrelom, csPrelom);
			mgdcTexty.Add(csStoleti, csStoleti);
			mgdcTexty.Add(csZacatek, csZacatek);
			mgdcTexty.Add(csTretina, csTretina);
			mgdcTexty.Add(csCtvrtina, csCtvrtina);
			mgdcTexty.Add(csAnte, csAnte);
			mgdcTexty.Add(csPost, csPost);
		}

		public static string UrcitObdobiVzniku(IDatace mdtcDatace) {
			string sDatace = null;
			if (mdtcDatace.Stoleti != 0) {
				if (mdtcDatace.PolovinaStoleti == 2) {
					sDatace = String.Format("{0}50–{1}", (mdtcDatace.Stoleti / 100), mdtcDatace.Stoleti + 100);
				}
				else
					sDatace = String.Format("{0}–{1}50", mdtcDatace.Stoleti, (mdtcDatace.Stoleti / 100));
			}
			return sDatace;
		}


		public static Datace AnalyzovatDataci(string sSlovniPopis) {
			//return AnalyzujDataci(sSlovniPopis);
			return AnalyzujDataci2(sSlovniPopis);
		}

		private static Datace AnalyzujDataci2(string sSlovniPopis) {
			char[] chOddelovace = new char[] { '/', '–', ',' };
			Datace dt = new Datace();
			List<string> glsCoObsahuje = new List<string>();
			List<string> glsNeznamaSlova = new List<string>();
			ZjistiObsahANeznamaSlova(sSlovniPopis, ref glsCoObsahuje, ref glsNeznamaSlova);

			List<TextovyPrvek> gltpTextovePrvky = new TextovePrvky(sSlovniPopis);
			List<string> glsNeznameVyrazy = new List<string>();
			foreach (TextovyPrvek tp in gltpTextovePrvky) {
				if (tp.Typ == TypTextovehoPrvku.Neurceno)
					glsNeznameVyrazy.Add(tp.Text);
			}

			if (glsNeznameVyrazy.Count > 0) {
				dt.Upresneni = "neznámé výrazy (" + String.Join("; ", glsNeznameVyrazy.ToArray()) + ")";
				return dt;
			}

			//if (glsNeznamaSlova.Count > 0) {
			//  dt.Upresneni = "neznámé výrazy (" + String.Join("; ", glsNeznamaSlova.ToArray()) + ")";
			//  return dt;
			//}

			string mstrSlovniPopis = sSlovniPopis;
			string sPopis = mstrSlovniPopis;

			if (glsCoObsahuje.Contains(csOtaznik)) {
				dt.Upresneni = csOtaznik;
				glsCoObsahuje.Remove(csOtaznik);
				sPopis = sPopis.Replace(csOtaznik, "").Trim();
			}

			string[] asRozhrani = null;
			if (sSlovniPopis.IndexOfAny(chOddelovace) > 0) {
				asRozhrani = sPopis.Split(chOddelovace);
			}

			if (glsCoObsahuje.Contains(csNebo)) {
				asRozhrani = sPopis.Split(new string[] { csNebo }, StringSplitOptions.RemoveEmptyEntries);
			}
			if (asRozhrani != null) {
				Datace dtZacatek = new Datace(asRozhrani[0].Trim());
				Datace dtKonec = new Datace(asRozhrani[1].Trim());
				dt = dtKonec;
				dt.NePredRokem = dtZacatek.NePredRokem;
				if (dtZacatek.Upresneni != null)
				{
					if (dtKonec.Upresneni != null)
						dt.Upresneni = String.Format("{0}; {1}", dtZacatek.Upresneni, dtKonec.Upresneni);
					else
						dt.Upresneni = dtZacatek.Upresneni;
				}

				return dt;
			}

			if (glsCoObsahuje.Contains(csPrelom)) {
				//vypreparovat první a druhé století
				sPopis = sPopis.Remove(sPopis.IndexOf(csPrelom), csPrelom.Length + 1);
				asRozhrani = sPopis.Split(new string[] { " a " }, StringSplitOptions.RemoveEmptyEntries);
				dt.RelativniChronologie = 0;
				Datace dtKonec = new Datace(asRozhrani[1]);
				Datace dtZacatek = new Datace();
				dtZacatek.Stoleti = dtKonec.Stoleti - cintStoLet;
				dtZacatek.RelativniChronologie = 9;
				dtZacatek.NePredRokem = dtZacatek.Stoleti + (cintStoLet - cintPrelom);
				dt.Stoleti = dtKonec.Stoleti;
				dt.PolovinaStoleti = dtKonec.PolovinaStoleti;
				dt.NePredRokem = dtZacatek.NePredRokem;
				dt.NePoRoce = dtZacatek.Stoleti + (cintStoLet + cintPrelom);
				dt.Upresneni = csPrelom;

			}

			if (glsCoObsahuje.Contains(csStoleti)) {
				//vypreparovat století, analayzovat zbytek
				int iStoleti = ZjistiStoleti(sPopis);
				dt.Stoleti = iStoleti;

				sPopis = sPopis.Replace(((iStoleti / cintStoLet) + 1).ToString() + csStoleti, "").Trim();
				if (glsCoObsahuje.Contains(csLeta) ||
					glsCoObsahuje.Contains(csPolovina) ||
					glsCoObsahuje.Contains(csTretina) ||
					glsCoObsahuje.Contains(csCtvrtina) ||
					glsCoObsahuje.Contains(csJenPolovina)) {

					CasovyZlomek zl = ZjistiZlomek(sPopis);
					sPopis = sPopis.Replace(zl.CitatelPopis + zl.JmenovatelPopis, "").Trim();

					if (zl.Jmenovatel == 10) {
						dt.Desetileti = zl.Citatel;
						dt.PolovinaStoleti = PolovinaStoletiNaZakladeDesetileti(dt.Desetileti);
						dt.NePredRokem = dt.Stoleti + zl.Jmenovatel * zl.Citatel;
						dt.NePoRoce = dt.NePredRokem + zl.Jmenovatel;
					}
					else if (zl.Jmenovatel == 2 && zl.Citatel == -1) {

						dt.PolovinaStoleti = 0;
						dt.RelativniChronologie = 5;
						dt.Upresneni = csJenPolovina;

						dt.NePredRokem = dt.Stoleti + (50) - cintJenPolovina;
						dt.NePoRoce = dt.NePredRokem + 50;
					}
					else {
						dt.Stoleti = iStoleti;
						dt.PolovinaStoleti = PolovinaStoletiNaZakladeDesetileti(((cintStoLet / zl.Jmenovatel) * (zl.Citatel)));
						dt.Desetileti = ((cintStoLet / zl.Jmenovatel) * (zl.Citatel)) / 10;

						dt.NePredRokem = dt.Stoleti + ((cintStoLet / zl.Jmenovatel) * (zl.Citatel - 1));
						dt.NePoRoce = dt.Stoleti + ((cintStoLet / zl.Jmenovatel) * (zl.Citatel));

						RelativniChronologieNaZakladeRoku(dt.NePoRoce);
					}

				}
				else if (glsCoObsahuje.Count == 1) {
					dt.RelativniChronologie = 0;
					dt.NePredRokem = dt.Stoleti;
					dt.NePoRoce = dt.Stoleti + cintStoLet;
				}
			}
			if (sPopis == csZacatek) {
				dt.RelativniChronologie = 1;
				dt.NePredRokem = dt.Stoleti;
				dt.NePoRoce = dt.Stoleti + cintZacatekKonec;
				dt.Upresneni = csZacatek;
				sPopis = sPopis.Replace(csZacatek, "").Trim();
			}
			if (sPopis == csKonec) {
				dt.NePoRoce = dt.Stoleti + cintStoLet;
				dt.NePredRokem = dt.Stoleti + cintStoLet - cintZacatekKonec;
				dt.RelativniChronologie = 9;
				dt.Upresneni = csKonec;
				sPopis = sPopis.Replace(csKonec, "").Trim();

			} 

			if (glsCoObsahuje.Contains(csPoRoce))
			{
				sPopis = sPopis.Remove(sPopis.IndexOf(csPoRoce), csPoRoce.Length);
				dt.Upresneni = csPoRoce;
				int iRok;
				if (Int32.TryParse(sPopis, out iRok))
				{
					UrciDataciNaZakladeRoku(dt, iRok);
					dt.NePoRoce = dt.Rok + cintOkoloRoku; //TODO Tady by mělo být koncové datum, např. úmrtí autora
					dt.NePredRokem = dt.Rok;
				}
			}
			if (glsCoObsahuje.Contains(csOkoloRoku)) {
				sPopis = sPopis.Remove(sPopis.IndexOf(csOkoloRoku), csOkoloRoku.Length);
				dt.Upresneni = csOkoloRoku;
				int iRok;
				if (Int32.TryParse(sPopis, out iRok)) {
					UrciDataciNaZakladeRoku(dt, iRok);
					dt.NePoRoce = dt.Rok + cintOkoloRoku;
					dt.NePredRokem = dt.Rok - cintOkoloRoku;
				}
			}

			if (glsCoObsahuje.Contains(csPost))
			{
				sPopis = sPopis.Remove(sPopis.IndexOf(csPost), csPost.Length + 1);
				dt.Upresneni = csPost;
				int iRok;
				if (Int32.TryParse(sPopis.Substring(0, 4), out iRok))
				{
					UrciDataciNaZakladeRoku(dt, iRok);
					dt.NePoRoce = iRok;
				}
			}

			if (glsCoObsahuje.Contains(csAnte))
			{
				sPopis = sPopis.Remove(sPopis.IndexOf(csAnte), csAnte.Length + 1);
				if (!String.IsNullOrEmpty(dt.Upresneni))
				{
					dt.Upresneni += "; " + csAnte;
				}
				else
				{
					dt.Upresneni = csAnte;
				}
				int iRok;
				if (Int32.TryParse(sPopis.Substring(0, 4), out iRok))
				{
					UrciDataciNaZakladeRoku(dt, iRok);
					dt.NePoRoce = iRok;
				}				
			}

			//jde pouze o rok
			if (glsCoObsahuje.Count == 0) {
				int iRok;
				if (Int32.TryParse(sPopis, out iRok)) {
					UrciDataciNaZakladeRoku(dt, iRok);
				}
			}
			return dt;

		}


		private static CasovyZlomek ZjistiZlomek(string sPopis) {
			CasovyZlomek zl = new CasovyZlomek();
			string sJmenovatel = csPolovina;

			int iPozice = sPopis.IndexOf(sJmenovatel);
			if (iPozice == -1) {
				sJmenovatel = csTretina;
				iPozice = sPopis.IndexOf(sJmenovatel);
			}
			if (iPozice == -1) {
				sJmenovatel = csCtvrtina;
				iPozice = sPopis.IndexOf(sJmenovatel);
			}
			if (iPozice == -1) {
				sJmenovatel = csLeta;
				iPozice = sPopis.IndexOf(sJmenovatel);
			}
			if (iPozice == -1) {
				sJmenovatel = csJenPolovina;
				iPozice = sPopis.IndexOf(sJmenovatel);
			}
			if (iPozice == -1)
				return zl;
			zl.JmenovatelPopis = sJmenovatel;
			zl.Jmenovatel = CasovyZlomek.JmenovatelPopisNaCislo(sJmenovatel);

			string sCitatel = null;
			if (sJmenovatel == csLeta) {
				sCitatel = sPopis.Substring(sPopis.IndexOf(sJmenovatel) - 2, 2);
			}
			else if (sJmenovatel == csJenPolovina) {
				sCitatel = "";
			}
			else {
				sCitatel = sPopis.Substring(sPopis.IndexOf(sJmenovatel) - 1, 2);
			}
			if (sCitatel.EndsWith("."))
				sCitatel = sCitatel.Substring(0, sCitatel.Length - 1);
			zl.CitatelPopis = sCitatel;
			if (sCitatel == "")
				zl.Citatel = 0;
			zl.Citatel = CasovyZlomek.CitatelPopisNaCislo(sCitatel);
			return zl;

		}

		private static int ZjistiStoleti(string sPopis) {
			int iPozice = sPopis.IndexOf(csStoleti);
			if (iPozice > 0) {
				//předpokládá se dvoumístný údaj o století - rozšířit i na 9. století a níže
				string sText = sPopis.Substring(iPozice - 2, 2).Trim();
				int iStoleti;
				if (Int32.TryParse(sText, out iStoleti)) {
					return (iStoleti - 1) * 100;
				}
			}
			return -1;
		}
		static Datace AnalyzujDataci(string sSlovniPopis) {
			Datace dt = new Datace();
			List<string> glsCoObsahuje = new List<string>();
			List<string> glsNeznamaSlova = new List<string>();
			AnalyzatorDatace.ZjistiObsahANeznamaSlova(sSlovniPopis, ref glsCoObsahuje, ref glsNeznamaSlova);

			if (glsNeznamaSlova.Count > 0) {
				dt.Upresneni = "neznámé výrazy (" + String.Join("; ", glsNeznamaSlova.ToArray()) + ")";
				return dt;
			}

			string mstrSlovniPopis = sSlovniPopis;
			string sPopis = mstrSlovniPopis;
			int iPocatek;

			RozebratUpresneniVPopisu(ref sPopis, csOtaznik, ref dt);
			RozebratUpresneniVPopisu(ref sPopis, csOkoloRoku, ref dt);
			RozebratUpresneniVPopisu(ref sPopis, csPoRoce, ref dt);
			if (sPopis.Contains(csPrelom)) {
				dt.RelativniChronologie = 1;
				RozebratUpresneniVPopisu(ref sPopis, csPrelom, ref dt);
			}

			//datace typu 1578 nebo 1579
			if (sPopis.Contains(csNebo)) {
				string[] asRozhrani = sPopis.Split(new string[] { csNebo }, StringSplitOptions.RemoveEmptyEntries);
				sPopis = asRozhrani[asRozhrani.Length - 1];
				if (Int32.TryParse(asRozhrani[0], out iPocatek)) {
					dt.NePredRokem = iPocatek;
				}
				else {
					dt = new Datace(asRozhrani[0]);
					dt.AnalyzovatSlovniPopis(asRozhrani[0]);
					dt.NePredRokem = dt.NePredRokem;
				}
			}

			//datace typu 1432/1433
			if (sPopis.Contains("/")) {
				string[] asRozhrani = sPopis.Split(new char[] { '/' });
				sPopis = asRozhrani[asRozhrani.Length - 1];
				if (Int32.TryParse(asRozhrani[0], out iPocatek)) {
					dt.NePredRokem = iPocatek;
				}
			}
			//datace typu 1502–1503
			if (sPopis.Contains("–")) {
				string[] asRozhrani = sPopis.Split(new char[] { '–' });
				sPopis = asRozhrani[asRozhrani.Length - 1];
				if (Int32.TryParse(asRozhrani[0], out iPocatek)) {
					dt.NePredRokem = iPocatek;
				}
			}

			int iRok;
			if (Int32.TryParse(sPopis, out iRok)) {
				UrciDataciNaZakladeRoku(dt, iRok);

			}
			else {
				sPopis = mstrSlovniPopis;
			}

			//doplnit zpracovní csNebo
			//rozdělit řetězec na 2 části, zpracovat dataci a vybrat tu pozdější

			if (sPopis.Contains(csPrelom)) {
				dt.PolovinaStoleti = 1;
				sPopis = sPopis.Replace(csPrelom, "");
				if (sPopis.Contains(csA)) {
					sPopis = sPopis.Substring(sPopis.IndexOf(csA) + csA.Length);
				}
				dt.RelativniChronologie = 1;
			}

			if (sPopis.Contains(cs1polovina)) {
				dt.PolovinaStoleti = 1;
				dt.RelativniChronologie = 3;
				sPopis = sPopis.Replace(cs1polovina, "");
			}
			if (sPopis.Contains(cs2polovina)) {
				dt.PolovinaStoleti = 2;
				//Desetileti = 5;???
				dt.RelativniChronologie = 7;
				sPopis = sPopis.Replace(cs2polovina, "");
			}

			int iPozice;
			int iRozsah;
			int iKolikata;
			if (sPopis.Contains(csCtvrtina)) {
				iRozsah = 25;
				iPozice = sPopis.IndexOf(csCtvrtina);
				if (Int32.TryParse(sPopis.Substring(iPozice - 1, 1), out iKolikata)) {
					dt.PolovinaStoleti = PolovinaStoletiNaZakladeDesetileti((iKolikata * iRozsah - 1));
					dt.Desetileti = (iKolikata * iRozsah - 1) / 10;
					dt.RelativniChronologie = RelativniChronologieNaZakladeRoku(iKolikata * iRozsah - 1);
					//dt.NePredRokem = dt.Stoleti + (iKolikata * iKolikata);
					//dt.NePoRoce = dt.Stoleti + (iKolikata * iKolikata) + iRozsah;
					sPopis = sPopis.Remove(iPozice - 1, csCtvrtina.Length + 2);

				}
			}
			if (sPopis.Contains(csTretina)) {
				iRozsah = 33;
				iPozice = sPopis.IndexOf(csTretina);
				if (Int32.TryParse(sPopis.Substring(iPozice - 1, 1), out iKolikata)) {
					dt.PolovinaStoleti = PolovinaStoletiNaZakladeDesetileti(((iKolikata * iRozsah) - 1));
					dt.Desetileti = ((iKolikata * iRozsah) - 10) / 10;
					dt.RelativniChronologie = RelativniChronologieNaZakladeRoku(iKolikata * iRozsah - 10);
					//dt.NePredRokem = dt.Stoleti + (iKolikata * iRozsah);
					//dt.NePoRoce = dt.Stoleti + (iKolikata * iRozsah) + iRozsah;
					sPopis = sPopis.Remove(iPozice - 1, csTretina.Length + 2);
				}

			}

			iPozice = sPopis.IndexOf(csLeta);
			if (iPozice > 0) {
				string sText = sPopis.Substring(iPozice - 2, 2);
				int iDesetileti;
				if (Int32.TryParse(sText, out iDesetileti)) {
					dt.PolovinaStoleti = PolovinaStoletiNaZakladeDesetileti(iDesetileti);
					dt.Desetileti = iDesetileti / 10;
					dt.RelativniChronologie = RelativniChronologieNaZakladeRoku(dt.Desetileti * 10 + 9);
					//dt.NePredRokem = dt.Stoleti + (iDesetileti);
					//dt.NePoRoce = dt.Stoleti + (iDesetileti) + iDesetileti;

					sPopis = sPopis.Remove(iPozice - 2, csLeta.Length + 2);
				}
			}

			iPozice = sPopis.IndexOf(csStoleti);
			if (iPozice > 0) {
				//předpokládá se dvoumístný údaj o století - rozšířit i na 9. století a níže
				string sText = sPopis.Substring(iPozice - 2, 2);
				int iStoleti;
				if (Int32.TryParse(sText, out iStoleti)) {
					dt.Stoleti = (iStoleti - 1) * 100;
					if (dt.RelativniChronologie == 0) {
						dt.RelativniChronologie = 9;
						dt.NePredRokem = dt.Stoleti;
						dt.NePoRoce = dt.Stoleti + (100);

					}

					//if (iRozsah != 0) { 

					//}
					if (dt.RelativniChronologie == 2 && dt.Desetileti == 2) {
						dt.NePredRokem = dt.Stoleti;
						dt.NePoRoce = dt.Stoleti + (dt.Desetileti * 10) + (int)(dt.RelativniChronologie * 12.5 - 1);
					}
					if (dt.RelativniChronologie == 4 && dt.Desetileti == 4) {
						dt.NePredRokem = dt.Stoleti + (dt.Desetileti * 10) - (int)(25);
						dt.NePoRoce = dt.Stoleti + (dt.Desetileti * 10) + (int)(25);
					}
					if (dt.RelativniChronologie == 8 && dt.Desetileti == 9) {
						dt.NePredRokem = dt.Stoleti + (25 * 3);
						dt.NePoRoce = dt.Stoleti + (25 * 4);
					}
					if (dt.RelativniChronologie == 8 && dt.Desetileti == 9) {
						dt.NePredRokem = dt.Stoleti + (dt.Desetileti * 10);
						dt.NePoRoce = dt.Stoleti + (dt.Desetileti * 10) + 10;
					}
					if (dt.RelativniChronologie == 3) {
						dt.NePredRokem = dt.Stoleti;
						dt.NePoRoce = dt.Stoleti + (dt.PolovinaStoleti * 50);
					}
					if (dt.RelativniChronologie == 7) {
						dt.NePredRokem = dt.Stoleti + (50);
						dt.NePoRoce = dt.Stoleti + 100;
					}
					if (dt.RelativniChronologie == 1) {
						dt.NePredRokem = dt.Stoleti - (int)(12.5);
						dt.NePoRoce = dt.Stoleti + (int)(12.5);
					}

					sPopis = sPopis.Remove(iPozice - 2, csStoleti.Length + 2);
				}
			}

			sPopis = sPopis.Trim();
			if (sPopis == csPolovina) {
				dt.Desetileti = 5;
				dt.PolovinaStoleti = 2;
				dt.RelativniChronologie = 5;

				dt.NePredRokem = dt.Stoleti + (50 - 25);
				dt.NePoRoce = dt.Stoleti + (50 + 25);

				sPopis = null;
			}

			switch (sPopis) {
				case csZacatek:
					dt.PolovinaStoleti = 1;
					dt.RelativniChronologie = 2;

					dt.NePredRokem = dt.Stoleti;
					dt.NePoRoce = dt.Stoleti + (int)(dt.RelativniChronologie * 12.5);

					dt.Upresneni = String.IsNullOrEmpty(dt.Upresneni) ? sPopis : dt.Upresneni + " " + sPopis;
					break;
				case csKonec:
					dt.PolovinaStoleti = 2;
					dt.RelativniChronologie = 8;

					dt.NePredRokem = dt.Stoleti + 100 - (int)(12.5);
					dt.NePoRoce = dt.Stoleti + 100;
					break;
				case csPolovina:
					dt.RelativniChronologie = 5;
					//dt.NePredRokem = dt.Stoleti;
					//dt.NePoRoce = dt.Stoleti * (dt.RelativniChronologie * 12.5);

					dt.Upresneni = String.IsNullOrEmpty(dt.Upresneni) ? sPopis : dt.Upresneni + " " + sPopis;
					break;
			}

			if (dt.Stoleti == 0) {
				if (!String.IsNullOrEmpty(sPopis)) {
					dt.Upresneni = sPopis;
				}
				else {
					int iCislo;
					if (!Int32.TryParse(sPopis, out iCislo))
						dt.Upresneni += " " + sPopis;
				}
			}
			return dt;
		}

		private static void UrciDataciNaZakladeRoku(IDatace dt, int iRok) {
			dt.Rok = iRok;
			dt.Stoleti = (iRok / 100) * 100;
			int iDesetileti = dt.Rok - dt.Stoleti;
			dt.PolovinaStoleti = PolovinaStoletiNaZakladeDesetileti(iDesetileti);
			dt.Desetileti = iDesetileti / 10;
			dt.RelativniChronologie = RelativniChronologieNaZakladeRoku(iRok);
			if (dt.NePredRokem == 0)
				dt.NePredRokem = dt.Rok;
			dt.NePoRoce = dt.Rok;
		}

		private static void ZjistiObsahANeznamaSlova(string sSlovniPopis, ref List<string> glsCoObsahuje, ref List<string> glNeznamaSlova) {
			StringBuilder sbSlovniPopis = new StringBuilder(sSlovniPopis);

			foreach (string sval in mgdcTexty.Values) {
				while (sbSlovniPopis.ToString().Contains(sval)) {
					int i = sbSlovniPopis.ToString().IndexOf(sval);
					sbSlovniPopis.Remove(i, sval.Length);
					glsCoObsahuje.Add(sval);
				}
			}

			const string csPismena = "0123456789/–.";
			string[] asSlova = sbSlovniPopis.ToString().Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (string sSlovo in asSlova) {
				for (int i = 0; i < sSlovo.Length; i++) {
					if (!(csPismena.Contains(sSlovo[i].ToString()))) {
						glNeznamaSlova.Add(sSlovo);
						break;
					}
				}
			}
		}

		private static int PolovinaStoletiNaZakladeDesetileti(int iDesetileti) {
			return iDesetileti < 50 ? 1 : 2;
		}

		private static int RelativniChronologieNaZakladeRoku(int iRok) {
			int iStoleti = (iRok / 100) * 100;
			int iDesetileti = iRok - iStoleti;
			int iTemp = (int)(Math.Truncate(iDesetileti / 12.5)) + 1; //snad to bude fungovat; ve VB Fix
			return iTemp;
		}


		private static void RozebratUpresneniVPopisu(ref string sPopis, string sUpresneni, ref Datace dtDatace) {
			if (sPopis.Contains(sUpresneni)) {
				dtDatace.Upresneni = String.IsNullOrEmpty(dtDatace.Upresneni) ? sUpresneni : dtDatace.Upresneni + " " + sUpresneni;
				sPopis = sPopis.Replace(sUpresneni, "").Trim();
			}
		}
	}
}
