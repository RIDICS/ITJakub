using System;
using Daliboris.Texty.Evidence.Rozhrani;
using System.Collections.Generic;

namespace Daliboris.Texty.Evidence {
	/// <summary>
	/// Pomocná třída pro filtrování a vyhledávání záznamů
	/// </summary>
	public class FiltrPrepisu {
		/// <summary>
		/// Identifikuje přepisy, které jsou určeny pro Manuscriptorium
		/// </summary>
		/// <param name="prp">Přepis</param>
		/// <returns>true pokud je přepis určen pro Manuscriptorium, false pokud není určen pro Manuscriptorium</returns>
		/// 


	  public string NazevSoboruBezPripony { get; set; }
		public string NazevSoboru { get; set; }
		public string GUID { get; set; }
		public string Text { get; set; }
		public int RokExportu { get; set; }
		public StatusAktualizace Status { get; set; }
		private bool JeZpusobVyuzitiVicenasobny { get; set; }

		private ZpusobVyuziti zpusobVyuziti;
		public ZpusobVyuziti ZpusobVyuziti
		{
			get { return zpusobVyuziti; }
			set { zpusobVyuziti = value;
			JeZpusobVyuzitiVicenasobny = (Enum.Format(typeof(ZpusobVyuziti), value, "G").IndexOf(',') > -1);
			}
		}

		public FazeZpracovani FazeZpracovani { get; set; }

		private bool JeZabezpeceniVicenasobne { get; set; }
		private Zabezpeceni zabezpeceni;
		public Zabezpeceni Zabezpeceni {
		  get { return zabezpeceni; }
		  set {
			zabezpeceni = value;
			JeZabezpeceniVicenasobne = (Enum.Format(typeof(Zabezpeceni), value, "G").IndexOf(',') > -1);
		  }
		}



		public FiltrPrepisu() {}
	 /// <summary>
	 /// 
	 /// </summary>
	 /// <param name="strText">text vyskytující se v titulu, v názvu souboru nebo jako autor</param>
		public FiltrPrepisu(string  strText)
		{
			Text = strText;
		}
	 public FiltrPrepisu(string text, string nazevSouboru) : this(text)
	 {
		NazevSoboru = nazevSouboru;
	 }

	 public FiltrPrepisu(Zabezpeceni zabezpeceni) {
	   Zabezpeceni = zabezpeceni;
	 }

		public FiltrPrepisu(ZpusobVyuziti zpZpusobVyuziti)
		{
			ZpusobVyuziti = zpZpusobVyuziti;
		}

		public FiltrPrepisu(ZpusobVyuziti zpusobVyuziti, Zabezpeceni zabezpeceni) : this(zpusobVyuziti)
		{
			Zabezpeceni = zabezpeceni;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public FiltrPrepisu(FazeZpracovani fazeZpracovani)
		{
			FazeZpracovani = fazeZpracovani;
		}

		public bool PodleNazvuSouboruBezPripony(Prepis prp) {
		  return (prp.Soubor.NazevBezPripony == NazevSoboruBezPripony);
		}
		public bool PodleNazvuSouboru(Prepis prp) {
			return (prp.NazevSouboru == NazevSoboru);
		}

		public bool PodleGUID(Prepis prp) {
			return (prp.Zpracovani.GUID == GUID);
		}

		public static bool PrepisyProManuscriptorium(Prepis prp) {
			IZpracovani z = prp.Zpracovani;
			if ((z.ZpusobVyuziti & ZpusobVyuziti.Manuscriptorium) == ZpusobVyuziti.Manuscriptorium) {
				return true;
			}
			else
				return false;
		}

		public bool PodleFazeZpracovani(Prepis prepis)
		{
			return (prepis.FazeZpracovani == FazeZpracovani);
		}

		public bool PodleMinimalniFazeZpracovani(Prepis prepis)
		{
			if (prepis.Neexportovat) return false;
			return (prepis.FazeZpracovani >= FazeZpracovani);
		}

		public bool PodleStatutu(Prepis prp) {
			return (prp.Status == Status);
		}

		public static bool FormalniKontrola(Prepis prp) {
			return (prp.Zpracovani.FazeZpracovani == FazeZpracovani.FormalniKontrola);
		}

		public static bool NeznamaDatace(Prepis prp) {
			if (prp.Hlavicka.DataceDetaily.Upresneni == null)
				return false;
			return prp.Hlavicka.DataceDetaily.Upresneni.Contains("neznámé výrazy");
		}

		public static bool MaEdicniPoznamku(Prepis prp) {
		  return (prp.Zpracovani.MaEdicniPoznamku);
		}

		public static bool NezadanaDatace(Prepis prp) {
			return String.IsNullOrEmpty(prp.Hlavicka.DataceText);
		}

		/// <summary>
		/// Identifikuje přepisy, které nejsou spolehlivě přepsané
		/// </summary>
		/// <param name="prp">Přepis</param>
		/// <returns>true pokud je přepis určen pro export do interního korpusu, false pokud není určen pro export do interního korpusu</returns>
		public static bool PrepisyNespolehlive(Prepis prp) { 
			IZpracovani z = prp.Zpracovani;
			if (z.CasoveZarazeni == CasoveZarazeni.PoRoce2000 || z.CasoveZarazeni == CasoveZarazeni.Nezarazeno || z.Neexportovat) {
				return false;
			}
			else {
				if (z.Exporty.Count > 0)
					return false;
				return true;
			}

		}

	 /// <summary>
		/// Vyhodnotí přepis jako určený pro zadaný způsob využití (vlastnost ZpusobVyuziti), bez ohledu na fázi zpracování.
	 /// </summary>
	 /// <param name="prp"></param>
	 /// <returns></returns>
		public bool PrepisyProZpusobVyuziti(Prepis prp) {
		 IZpracovani z = prp.Zpracovani;
		 if ((z.ZpusobVyuziti & this.ZpusobVyuziti) == this.ZpusobVyuziti) {
			return true;
		 }
		 else
			return false;
		}

		public bool PrepisyPodleZabezpeceni(Prepis prepis) {
		  IZpracovani z = prepis.Zpracovani;
		  Zabezpeceni prava = z.Zabezpeceni;
		  string sFormat = Enum.Format(typeof(Zabezpeceni), this.Zabezpeceni, "G");
		  if (z.FazeZpracovani < FazeZpracovani.Exportovat || z.Neexportovat)
			return false;
		  if (JeZabezpeceniVicenasobne)
			return ((z.Zabezpeceni & this.Zabezpeceni) != 0);
		  if ((z.Zabezpeceni & this.Zabezpeceni) == this.Zabezpeceni)
			return true;
		  return false;
		}

		/// <summary>
		/// Vyhodnotí přepis jako určený pro export, který je shodný jako nastavení filtru (vlastnost ZpusobVyuziti).
		/// </summary>
		/// <param name="prp">Přepis, jehož vlastnost ZpusobVyuziti se porovnavá s toutéž vlastností nastavenou ve filtru.</param>
		/// <returns></returns>
		public bool PrepisyProExportDo(Prepis prp)
		{
			IZpracovani z = prp.Zpracovani;
			bool response = false;

			response = z.Neexportovat;

			if (response)
				return false;

			if (this.ZpusobVyuziti == ZpusobVyuziti.InterniKorpus)
				return PrepisyProExportDoInternihoKorpusu(prp);

			response = PrepisPodleZabezpeceni(z);
			if (!response)
				return false;

			response = PodleFazeZpracovani(z);
			if (!response)
				return false;

			response = PrepisPodleZpusobuVyuziti(z);

			return response;

			/*
			if ((z.Zabezpeceni & Zabezpeceni.Verejne) != Zabezpeceni.Verejne)
			  return false;
					if (z.FazeZpracovani < FazeZpracovani.Exportovat)
						return false;
					if (JeZpusobVyuzitiVicenasobny)
						return ((z.ZpusobVyuziti & this.ZpusobVyuziti) != 0);
			if ((z.ZpusobVyuziti & this.ZpusobVyuziti) == this.ZpusobVyuziti) 
				return true;
			return false;
			 * 
			 */ 
		}


		private bool PodleFazeZpracovani(IZpracovani zpracovani)
		{
			return zpracovani.FazeZpracovani >= FazeZpracovani.Exportovat;
		}

		/// <summary>
		/// Není-li zabezpečení nastaveno, nekontroluje se, takže projdou všechny soubory (s jakýmkoli zabezpečením).
		/// </summary>
		/// <param name="zpracovani"></param>
		/// <returns></returns>
		private bool PrepisPodleZabezpeceni(IZpracovani zpracovani)
		{
			if (this.Zabezpeceni == 0)
			{
				return true;
			}
			if (JeZabezpeceniVicenasobne)
			{
				return ((zpracovani.Zabezpeceni & this.Zabezpeceni) != 0);
			}
			else
				return (zpracovani.Zabezpeceni & this.Zabezpeceni) == this.Zabezpeceni;

		}

		/// <summary>
		/// Není-li definován způsob využití, soubor neprojde.
		/// </summary>
		/// <param name="zpracovani"></param>
		/// <returns></returns>
		private bool PrepisPodleZpusobuVyuziti(IZpracovani zpracovani)
		{
			if (this.ZpusobVyuziti == 0)
			{
				return false;
			}
			if (JeZpusobVyuzitiVicenasobny)
				return ((zpracovani.ZpusobVyuziti & this.ZpusobVyuziti) != 0);
			return (zpracovani.ZpusobVyuziti & this.ZpusobVyuziti) == this.ZpusobVyuziti;
		}


	 public static bool PrepisyProExportKamkoli(Prepis prp)
	 {
		Zpracovani z = prp.Zpracovani;
		if (z.CasoveZarazeni == CasoveZarazeni.PoRoce2000 || z.CasoveZarazeni == CasoveZarazeni.Nezarazeno || z.Neexportovat)
			return false;
		if (prp.FazeZpracovani >= FazeZpracovani.Exportovat)
			return true;
		return false;
	 }

		/// <summary>
		/// Identifikuje přepisy, které jsou určeny pro export do interního korpusu
		/// </summary>
		/// <param name="prp">Přepis</param>
		/// <returns>true pokud je přepis určen pro export do interního korpusu, false pokud není určen pro export do interního korpusu</returns>
		public static bool PrepisyProExportDoInternihoKorpusu(Prepis prp) {
			IZpracovani z = prp.Zpracovani;

			
			if ((z.ZpusobVyuziti & Rozhrani.ZpusobVyuziti.InterniKorpus) != 0)
				return true;
			else
			{
				return false;
			}
			if ( z.CasoveZarazeni == CasoveZarazeni.PoRoce2000 || z.CasoveZarazeni == CasoveZarazeni.Nezarazeno || z.Neexportovat) {
				return false;
			}
			else
				return true;
		}

		/// <summary>
		/// Identifikuje dosud neexportované přepisy, které jsou určeny pro export do interního korpusu
		/// </summary>
		/// <param name="prp">Přepis</param>
		/// <returns>true pokud je přepis určen pro export do interního korpusu, false pokud není určen pro export do interního korpusu</returns>
		public static bool PrepisyProExportDoInternihoKorpusuDosudNeexportovane(Prepis prp) {
			IZpracovani z = prp.Zpracovani;
			if (PrepisyProExportDoInternihoKorpusu(prp) && z.Exporty.Count == 0 ) {
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Identifikuje přepisy, které jsou určeny pro export do Manuscriptoria
		/// </summary>
		/// <param name="prp">Přepis</param>
		/// <returns>true pokud je přepis určen pro export do Manuscriptoria, false pokud není určen pro export do Manuscriptoria</returns>
		public static bool PrepisyProExportDoManuscriptoria(Prepis prp) {
			IZpracovani z = prp.Zpracovani;
			if ((z.ZpusobVyuziti & ZpusobVyuziti.Manuscriptorium) == ZpusobVyuziti.Manuscriptorium & z.FazeZpracovani >= FazeZpracovani.Exportovat & !z.Neexportovat) {
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Identifikuje přepisy, které jsou určeny pro export do elektronické knihy
		/// </summary>
		/// <param name="prp">Přepis</param>
		/// <returns>true pokud je přepis určen pro export do elektronické knihy, false pokud není určen pro export do elektronické knihy</returns>
		public static bool PrepisyProExportDoElektronickeKnihy(Prepis prp) {
			IZpracovani z = prp.Zpracovani;
			if ((z.ZpusobVyuziti & ZpusobVyuziti.ElektronickaKniha) == ZpusobVyuziti.ElektronickaKniha && z.FazeZpracovani >= FazeZpracovani.Exportovat & !z.Neexportovat) {
				return true;
			}
			return false;
		}


		/// <summary>
		/// Identifikuje přepisy, které jsou určeny pro staročeskou textovou banku
		/// </summary>
		/// <param name="prp">Přepis</param>
		/// <returns>true pokud je přepis určen pro staročeskou textovou banku, false pokud není určen pro staročeskou textovou banku</returns>
		public static bool PrepisyProExportDoStaroceskehoKorpusu(Prepis prp) {
			IZpracovani z = prp.Zpracovani;
			if ((z.ZpusobVyuziti & ZpusobVyuziti.StaroceskyKorpus) == ZpusobVyuziti.StaroceskyKorpus & z.FazeZpracovani >= FazeZpracovani.Exportovat & !z.Neexportovat) {
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Identifikuje přepisy, které byly exportovány do staročeské textové banky nebo do Manuscriptoria 
		/// </summary>
		/// <param name="prp">Přepis</param>
		/// <returns></returns>
		public static bool ExportovanePrepisy(Prepis prp) {
			IZpracovani z = prp.Zpracovani;
			IExport expm = z.ZjistiPrvniExport(ZpusobVyuziti.Manuscriptorium);
			IExport exps = z.ZjistiPrvniExport(ZpusobVyuziti.StaroceskyKorpus);
			if ((expm != null) || (exps != null))
				return true;
			else
				return false;
		}

		public bool PodleExportuVerejnehoZaRok(Prepis prp) {
		  IZpracovani z = prp.Zpracovani;

		  if (!((z.Zabezpeceni & Rozhrani.Zabezpeceni.Verejne) == Rozhrani.Zabezpeceni.Verejne))
			return false;

		  bool vRoce = false;
		  List<IExport> exporty = new List<IExport>();
		  exporty.Add(z.ZjistiPrvniExport(ZpusobVyuziti.Manuscriptorium));
		  exporty.Add(z.ZjistiPrvniExport(ZpusobVyuziti.EdicniModul));
		  exporty.Add(z.ZjistiPrvniExport(ZpusobVyuziti.ElektronickaKniha));
		  exporty.Add(z.ZjistiPrvniExport(ZpusobVyuziti.StaroceskyKorpus));
		  foreach (IExport export in exporty) {
			if (export != null && export.CasExportu.Year == RokExportu)
			  vRoce = true;
			if (export != null && export.CasExportu.Year < RokExportu && export.ZpusobVyuziti != Rozhrani.ZpusobVyuziti.InterniKorpus)
			  return false;
		  }
		  return vRoce;
		}

		/// <summary>
		/// Identifikuje přepisy, které byly exportovány do staročeské textové banky nebo do Manuscriptoria v roce 2010
		/// </summary>
		/// <param name="prp">Přepis</param>
		/// <returns>true pokud byl přepis exportován do staročeské textové banky, false pokud nebyl do staročeské textové banky exportován</returns>
		public bool PodleExportuZaRok(Prepis prp) {
			IZpracovani z = prp.Zpracovani;
			bool vRoce = false;
			List<IExport> exporty = new List<IExport>();
			exporty.Add(z.ZjistiPrvniExport(ZpusobVyuziti.Manuscriptorium));
			exporty.Add(z.ZjistiPrvniExport(ZpusobVyuziti.EdicniModul));
			exporty.Add(z.ZjistiPrvniExport(ZpusobVyuziti.ElektronickaKniha));
			exporty.Add(z.ZjistiPrvniExport(ZpusobVyuziti.StaroceskyKorpus));
			foreach (IExport export in exporty) {
			  if (export != null && export.CasExportu.Year == RokExportu)
				vRoce = true;
			  if (export != null && export.CasExportu.Year < RokExportu && export.ZpusobVyuziti != Rozhrani.ZpusobVyuziti.InterniKorpus)
				return false;
			}
				return vRoce;
		}

		/// <summary>
		/// Identifikuje přepisy, které byly exportovány do staročeské textové banky
		/// </summary>
		/// <param name="prp">Přepis</param>
		/// <returns>true pokud byl přepis exportován do staročeské textové banky, false pokud nebyl do staročeské textové banky exportován</returns>
		public static bool PrepisyExportovaneDoStaroceskehoKorpusu(Prepis prp) {
			IZpracovani z = prp.Zpracovani;
			if ((z.ZpusobVyuziti & ZpusobVyuziti.StaroceskyKorpus) == ZpusobVyuziti.StaroceskyKorpus & z.FazeZpracovani >= FazeZpracovani.Exportovano & !z.Neexportovat) {
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Identifikuje přepisy, které byly exportovány do Manuscriptoria
		/// </summary>
		/// <param name="prp">Přepis</param>
		/// <returns>true pokud byl přepis exportován do staročeské textové banky, false pokud nebyl do staročeské textové banky exportován</returns>
		public static bool PrepisyExportovaneDoManuscriptoria(Prepis prp) {
			IZpracovani z = prp.Zpracovani;
			if ((z.ZpusobVyuziti & ZpusobVyuziti.Manuscriptorium) == ZpusobVyuziti.Manuscriptorium & z.FazeZpracovani >= FazeZpracovani.Exportovano & !z.Neexportovat) {
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Identifikuje přepisy, které jsou určeny pro export do staročeské textové banky
		/// </summary>
		/// <param name="prp">Přepis</param>
		/// <returns>true pokud je přepis určen pro export do staročeské textové banky, false pokud není určen pro export do staročeské textové banky</returns>
		public static bool PrepisyProStaroceskyKorpus(Prepis prp) {
			IZpracovani z = prp.Zpracovani;
			if ((z.ZpusobVyuziti & ZpusobVyuziti.StaroceskyKorpus) == ZpusobVyuziti.StaroceskyKorpus) {
				return true;
			}
			else
				return false;
		}

		public static bool PrepisyBezIdentifikatoru(Prepis prp) {
			return (String.IsNullOrEmpty(prp.Zpracovani.GUID));
		}
		public static bool ZmenenePrepisy(Prepis prp) {
			return (prp.Status == StatusAktualizace.Zmeneno);
		}
		public static bool OdstranenePrepisy(Prepis prp) {
			return (prp.Status == StatusAktualizace.Odstraneno);
		}
		public static bool NovePrepisy(Prepis prp) {
			return (prp.Status == StatusAktualizace.Nove);
		}
		public static bool NezmenenePrepisy(Prepis prp) {
			return (prp.Status == StatusAktualizace.BezeZmen);
		}

		public static bool PrepisyProHodnotitele(Prepis prp)
		{
			IZpracovani z = prp.Zpracovani;
			return ((z.Zabezpeceni & Zabezpeceni.Hodnotitele) == Zabezpeceni.Hodnotitele);
		}

		public bool ObsahujeText(Prepis prp) {
			bool bObsahuje = false;
			string sText = Text;
			if (sText == null)
				return bObsahuje;
			bool bRozlisovatVelikostPismen = false;

			if (!bRozlisovatVelikostPismen)
				sText = sText.ToLower();

			if (bRozlisovatVelikostPismen) {
				bObsahuje = prp.Titul != null && prp.Titul.Contains(sText);
				if (bObsahuje)
					return bObsahuje;

				bObsahuje = prp.Autor != null && prp.Autor.Contains(sText);
				if (bObsahuje)
					return bObsahuje;
				bObsahuje = prp.NazevSouboru != null && prp.NazevSouboru.Contains(sText);
				if (bObsahuje)
					return bObsahuje;
			}
			else {
				bObsahuje = prp.Titul != null && prp.Titul.ToLower().Contains(sText);
				if (bObsahuje)
					return bObsahuje;

				bObsahuje = prp.Autor != null && prp.Autor.ToLower().Contains(sText);
				if (bObsahuje)
					return bObsahuje;
				bObsahuje = prp.NazevSouboru != null && prp.NazevSouboru.ToLower().Contains(sText);
				if (bObsahuje)
					return bObsahuje;

			}
			return bObsahuje;
		}

	}

}
