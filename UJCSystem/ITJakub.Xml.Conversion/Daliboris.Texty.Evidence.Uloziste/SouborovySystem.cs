using Daliboris.Pomucky.Soubory.MetaInfo;
using Daliboris.Pomucky.Xml;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence.Uloziste {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Xml;
	using Daliboris.OOXML.Pomucky;
	using Daliboris.Pomucky.Soubory;
		using System.Runtime.InteropServices;
		using System.Text;




	public class SouborovySystem {
				[DllImport("shell32.dll", EntryPoint = "FindExecutable")]
				public static extern long FindExecutableA(string lpFile, string lpDirectory, StringBuilder lpResult);

		private string mstrSlozka;
		private string mstrMaska = "*.do*?";

		public delegate void Nacteni(object sender, NacteniPrepisuEventArgs ev);
		public event Nacteni NacteniSouboru;

		private int mintPocetSouboru;

				public static string FindExecutable(string pv_strFilename)
				{
						StringBuilder objResultBuffer = new StringBuilder(1024);
						long lngResult = 0;

						lngResult = FindExecutableA(pv_strFilename, string.Empty, objResultBuffer);

						if (lngResult >= 32)
						{
								return objResultBuffer.ToString();
						}

						return null;
				}


		public SouborovySystem(string strSlozka) {
			mstrSlozka = strSlozka;
		}

		protected void OnNacteniSouboru(object sender, NacteniPrepisuEventArgs ev) {
			if (NacteniSouboru != null)
				NacteniSouboru(sender, ev);
		}

		public string Slozka {
			get { return mstrSlozka; }
			set { mstrSlozka = value; }
		}

		public string MaskaSouboru {
			get { return mstrMaska; }
			set { mstrMaska = value; }

		}
		public Prepisy NacistPrepisy() {
			return NacistPrepisy(false);
		}
		public Prepisy NacistPrepisy(bool blnJenomZaklad) {
			DirectoryInfo di = new DirectoryInfo(mstrSlozka);
			if (String.IsNullOrEmpty(mstrMaska)) {
				mstrMaska = "*.do*?";
			}
			FileInfo[] fis = di.GetFiles(mstrMaska,  SearchOption.TopDirectoryOnly);
			Prepis[] dkm = new Prepis[fis.Length];
			Prepisy prp = new Prepisy();

			mintPocetSouboru = fis.Length;
			if(NacteniSouboru != null)
				NacteniSouboru(this, new NacteniPrepisuEventArgs(mintPocetSouboru));
			

			for (int i = 0; i < fis.Length; i++) {
				if (NacteniSouboru != null)
					NacteniSouboru(this, new NacteniPrepisuEventArgs(mintPocetSouboru, i+1, fis[i].Name));

				string sSouborDocx = null;
				ISoubor sb = new Evidence.Soubor();
				sb.Adresar = mstrSlozka;
				sb.Nazev = fis[i].Name;
				sb.Zmeneno = fis[i].LastWriteTime;
				sb.Velikost = fis[i].Length;
				if (fis[i].Extension.ToLower() == ".docx" | fis[i].Extension.ToLower() == ".docm") {
					sSouborDocx = sb.CelaCesta;
					sb.FormatWordu = FormatSouboru.Docx;
				}
				else if (fis[i].Extension.ToLower() == ".doc")
					sb.FormatWordu = FormatSouboru.Doc;
				else
					sb.FormatWordu = FormatSouboru.Neznamy;
				sb.KontrolniSoucet = KontrolniSoucet.GetMD5Hash(fis[i].FullName);

				if (!blnJenomZaklad) {
					if (sb.FormatWordu == FormatSouboru.Doc) {
						throw new FormatException("Sobory DOC nejsou podporovány");
						sSouborDocx = System.IO.Path.GetTempPath() + fis[i].Name + "x";
						//Konverze.Doc2Docx(fis[i].FullName, sSouborDocx);
						FileInfo fi = new FileInfo(sSouborDocx);
												if(fi.Exists)
								sb.Velikost = fi.Length;
					}
				}
				Hlavicka hl = new Hlavicka();
				Zpracovani zp = new Zpracovani();

				//object oGuid = Metadata.NactiUzivatelskouVlastnost(sb.CelaCesta, Enum.GetName(typeof(Metaudaje), Metaudaje.htx_id));
				//if (oGuid != null)
				//   dk.GUID = oGuid.ToString();
				string[] asMetaudaje = Enum.GetNames(typeof(Metaudaje));
				object[] aoUdaje = Metadata.NactiUzivatelskeVlastnosti(sb.CelaCesta, asMetaudaje);
				for (int j = 0; j < asMetaudaje.Length; j++) {
					if (aoUdaje[j] != null) {
						DateTime dtExport;


						switch (asMetaudaje[j]) {

							case "htx_id":
								zp.GUID = aoUdaje[j].ToString();
								break;
							case "htx_posledniExport_1":
								if (PrevestNaDatum(aoUdaje[j], out dtExport))
									zp.Exporty.Add(new Export(ZpusobVyuziti.Manuscriptorium, dtExport));
								break;
							case "htx_posledniExport_2":
								if (PrevestNaDatum(aoUdaje[j], out dtExport))
									zp.Exporty.Add(new Export(ZpusobVyuziti.StaroceskyKorpus, dtExport));
								break;
							case "htx_posledniExport_4":
								if (PrevestNaDatum(aoUdaje[j], out dtExport))
									zp.Exporty.Add(new Export(ZpusobVyuziti.StredoceskyKorpus, dtExport));
								break;
							case "htx_fazeZpracovani":
								zp.FazeZpracovani = (FazeZpracovani)Enum.Parse(typeof(FazeZpracovani), aoUdaje[j].ToString());
								break;
							case "ovj_casoveZarazeni":
								zp.CasoveZarazeni = (CasoveZarazeni)Enum.Parse(typeof(CasoveZarazeni), aoUdaje[j].ToString());
								break;
							case "ovj_zpusobVyuziti":
								zp.ZpusobVyuziti = (ZpusobVyuziti)Enum.Parse(typeof(ZpusobVyuziti), aoUdaje[j].ToString());
								break;
							case "htx_neexportovat":
								bool bNeexportovat;
								if (Boolean.TryParse(aoUdaje[j].ToString(), out bNeexportovat))
									zp.Neexportovat = bNeexportovat;
								else
									zp.Neexportovat = false;
								break;
							default:
								break;
						}
					}
				}
				asMetaudaje = new string[] { "Titul", "Předmět", "Komentář", "Autor" };
				aoUdaje = Metadata.NactiZabudovaneVlastnosti(sb.CelaCesta, asMetaudaje);
				for (int j = 0; j < asMetaudaje.Length; j++) {
					if (aoUdaje[j] != null) {
						switch (asMetaudaje[j]) {
							case "Titul":
								hl.Titul = aoUdaje[j].ToString();
								break;
							case "Předmět":
								hl.InstituceUlozeni = aoUdaje[j].ToString();
								break;
							case "Komentář":
								hl.Signatura = aoUdaje[j].ToString();
								break;
							case "Autor":
								hl.EditoriPrepisu = GetCleanEditori(aoUdaje[j].ToString());
								break;
							default:
								break;
						}
					}
				}

				string grantovaPodpora = null;
				if (!blnJenomZaklad)
				{
					NactiHlavickuZTabulky(sSouborDocx, ref hl);
					grantovaPodpora = ZjistiGrantovouPodporu(sSouborDocx);
				}
				
				Prepis dk = new Prepis(hl, sb, zp);
				//dk.Identifikator = i;
				dk.Zpracovani.GrantovaPodpora = grantovaPodpora;
				dkm[i] = dk;

				prp.Add(dk);


				if (dk.Soubor.CelaCesta != sSouborDocx & sSouborDocx != null)
					File.Delete(sSouborDocx);

			}
			return prp;
			//return dkm;
		}

		private static string[] GetCleanEditori(string editoriText)
		{
			string[] editori = editoriText.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < editori.Length; i++)
			{
				editori[i] = editori[i].Trim();
			}
			return editori;
		}
		private bool PrevestNaDatum(object oDatum, out DateTime dtDatum) {

			DateTime dt;
			//if (DateTime.TryParse(oDatum.ToString(), ci.DateTimeFormat, System.Globalization.DateTimeStyles.None, out dt)) {
			if (DateTime.TryParse(oDatum.ToString(), out dt)) {
				dtDatum = dt;
				return true;
			}
			else {
				dtDatum = dt;
				return false;
			}
		}

		private string ZjistiGrantovouPodporu(string souborDocx)
		{
			string result = null;
			if (!File.Exists(souborDocx))
				return result;
			string sTempXml = Path.GetTempFileName();
			//OOXML.Pomucky.Dokument.ExtrahovatDoSouboru(sSouborDocx, sTempXml, true);
			OOXML.Pomucky.Dokument.ExtrahovatDoSouboru(souborDocx, sTempXml, false);
			XmlReaderSettings xrs = new XmlReaderSettings();
			xrs.CloseInput = true;
			xrs.ConformanceLevel = ConformanceLevel.Document;
			XmlReader xr = XmlReader.Create(sTempXml, xrs);

			XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
			namespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			try
			{


				while (xr.Read())
				{
					if (xr.NodeType == XmlNodeType.Element)
					{
						switch (xr.Name)
						{
							case "w:p":
								XmlDocument paragraph = Objekty.ReadNodeAsXmlDocument(xr);
								XmlNode node = paragraph.SelectSingleNode("/w:p/w:pPr/w:pStyle", namespaceManager);
								if (node != null && node.Attributes.Count > 0 && 
									(node.Attributes["w:val"].Value == "Grantovapodpora" || node.Attributes["w:val"].Value == "Grantova_podpora"))
								{
									result = paragraph.InnerText;
									xr.Close();
									goto End;
								}
								break;
						}
					}
				}
			}
			catch (Exception e)
			{
				throw e;
			}
			finally
			{
				xr.Close();
				File.Delete(sTempXml);
			}
		End:
			if(File.Exists(sTempXml))
				File.Delete(sTempXml);
			return result;
		}
		private void NactiHlavickuZTabulky(string sSouborDocx, ref Hlavicka hl) {
			if (!File.Exists(sSouborDocx))
				return;
			string sTempXml = Path.GetTempFileName();
			//OOXML.Pomucky.Dokument.ExtrahovatDoSouboru(sSouborDocx, sTempXml, true);
			OOXML.Pomucky.Dokument.ExtrahovatDoSouboru(sSouborDocx, sTempXml, false);
			XmlReaderSettings xrs = new XmlReaderSettings();
			xrs.CloseInput = true;
			xrs.ConformanceLevel = ConformanceLevel.Document;
			XmlReader xr = XmlReader.Create(sTempXml, xrs);
			

			int iAktualniTabulka = 0;
			int iAktualniRadek = 0;
			int iAktualniSloupec = 0;
			string sText = null;
			List<string> glEditori = new List<string>();

			try {


				while (xr.Read()) {
					if (xr.NodeType == XmlNodeType.Element) {
						switch (xr.Name) {
							case "w:tbl":
								iAktualniRadek = 0;
								iAktualniSloupec = 0;
								iAktualniTabulka++;
								if (iAktualniTabulka > 1)
									return;
								sText = null;
								glEditori = new List<string>();
								break;
							case "w:tr":
								iAktualniRadek++;
								iAktualniSloupec = 0;
								break;
							case "w:tc":
								iAktualniSloupec++;
								sText = null;
								break;
							case "w:t":
								if (!xr.IsEmptyElement & iAktualniTabulka == 1)
									sText += xr.ReadElementContentAsString();
								break;
							default:
								break;
						}
					}
					else if (xr.NodeType == XmlNodeType.EndElement) {
						switch (xr.Name) {
							case "w:tbl":
								if (iAktualniTabulka >= 1) {
									if(glEditori.Count > 0)
										hl.EditoriPrepisu = glEditori.ToArray();
									return;
								}
								break;
							case "w:tc":
								if (sText == null || iAktualniSloupec != 2)
									break;
								sText = sText.Trim();
								if (!String.IsNullOrEmpty(sText)) {
									switch (iAktualniRadek) {
										case 1:
											hl.Autor = sText;
											break;
										case 2:
											hl.Titul = sText;
											break;
										case 3:
											hl.DataceText = sText;
											break;
										case 4:
											hl.Tiskar = sText;
											break;
										case 5:
											hl.MistoTisku = sText;
											break;
										case 7:
											hl.TypPredlohyText = sText;
											break;
										case 9:
											hl.ZemeUlozeni = sText;
											break;
										case 10:
											hl.MestoUlozeni = sText;
											break;
										case 11:
											hl.InstituceUlozeni = sText;
											break;
										case 12:
											hl.Signatura = sText;
											break;
										case 13:
											hl.FoliacePaginace = sText;
											break;
										case 15:
											hl.TitulEdice = sText;
											break;
										case 16:
											hl.EditorEdice = sText;
											break;
										case 17:
											hl.MistoVydaniEdice = sText;
											break;
										case 18:
											hl.RokVydaniEdice = sText;
											break;
										case 19:
											hl.StranyEdice = sText;
											break;
										case 21:
										case 22:
										case 23:
										case 24:
											glEditori.Add(sText);
											break;
										default:
											break;
									}
								}
								break;
							default:
								break;
						}
					}

				}
			}
			catch (Exception e) {
				throw e;
			}
			finally {
				xr.Close();
				File.Delete(sTempXml);
			}

		}

		public bool UlozitPrepis(Prepis prpPrepis) {
			IZpracovani zp = prpPrepis.Zpracovani;
			Dictionary<string, object> gdcVlastnosti = new Dictionary<string, object>();
			gdcVlastnosti.Add("htx_id", zp.GUID);
			gdcVlastnosti.Add("htx_fazeZpracovani", zp.FazeZpracovani);
			gdcVlastnosti.Add("ovj_casoveZarazeni", zp.CasoveZarazeni);
			gdcVlastnosti.Add("ovj_zpusobVyuziti", zp.ZpusobVyuziti);
			gdcVlastnosti.Add("htx_neexportovat", zp.Neexportovat);

			return Metadata.UlozUzivatelskeVlastnosti(prpPrepis.Soubor.CelaCesta, gdcVlastnosti);

		}
	}
}

