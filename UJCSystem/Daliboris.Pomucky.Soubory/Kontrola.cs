using System;
using System.Collections.Generic;
using System.IO;
namespace Daliboris.Pomucky.Soubory {

	public class Kontrola {
		private KontrolaNastaveni mknsNastaveni;
		private StreamWriter swLog;

		public KontrolaNastaveni Nastaveni {
			get { return mknsNastaveni; }
			set { mknsNastaveni = value; }
		}

		public Kontrola(KontrolaNastaveni knsNastaveni) {
			mknsNastaveni = knsNastaveni;
		}

		public void ZkontrolovatPrazdneSoubory() {
			using (swLog = new StreamWriter(mknsNastaveni.LogovaciSoubor)) {
				ZkontrolujPrazdneSoubory(mknsNastaveni.VychoziSlozka);
			}
		}

		private void ZkontrolujPrazdneSoubory(string strAdresar) {
			DirectoryInfo di = new DirectoryInfo(strAdresar);
			FileInfo[] fis = di.GetFiles();
			foreach (FileInfo fi in fis) {
				if (fi.Length == 0)
					swLog.WriteLine(fi.FullName);
			}
			if (mknsNastaveni.VcetnePodslozek) {
				DirectoryInfo[] dis = di.GetDirectories();
				foreach (DirectoryInfo d in dis) {
					ZkontrolujPrazdneSoubory(d.FullName);
				}
			}
		}

		public void PorovnatAdresare()
		{
			using (swLog = new StreamWriter(mknsNastaveni.LogovaciSoubor))
			{
				PorovnejAdresare(mknsNastaveni.VychoziSlozka, mknsNastaveni.CilovaSlozka);
			}
		}

		/// <summary>
		/// Kontroluje číselnou posloupnost v názvech souborů.
		/// Číselná posloupnost se kontroluje pouze v rámci jednhom adresáře.
		/// </summary>
		public void ZkontrolujPosloupnostCislovani()
		{
			using (swLog = new StreamWriter(mknsNastaveni.LogovaciSoubor))
			{
				DirectoryInfo di = new DirectoryInfo(mknsNastaveni.VychoziSlozka);
				ZkontrolujPosloupnostCislovaniVeSlozce(di);
				/*
				if(mknsNastaveni.VcetnePodslozek)
				{
					DirectoryInfo[] dis = di.GetDirectories();
					foreach (DirectoryInfo directoryInfo in dis)
					{
						ZkontrolujPosloupnostCislovaniVeSlozce(directoryInfo);
					}
				}
				 * */
			}

		}

		private void ZkontrolujPosloupnostCislovaniVeSlozce(DirectoryInfo diSlozka)
		{
			Console.WriteLine(diSlozka.FullName);

			FileInfo[] fis = diSlozka.GetFiles(mknsNastaveni.MaskaSouboru);
			FileComparer fc = new FileComparer();
			Array.Sort(fis, fc);
			int iPred = 0;
			for (int i = 0; i < fis.Length - 1; i++)
			{
				int iAkt;
				string sNazev = Soubor.NazevSouboruBezPripony(fis[i].FullName);
				if(!Int32.TryParse(sNazev, out iAkt))
				{
					swLog.WriteLine("{0} – neobsahuje číslo", fis[i].FullName);
				}
				else
				{
					if(iAkt != iPred + 1)
					{
						swLog.WriteLine("{0} – má být {1}, je {2}", fis[i].FullName, iPred + 1,	iAkt);
					}
					iPred = iAkt;
				}
			}
			if(mknsNastaveni.VcetnePodslozek)
				{
					DirectoryInfo[] dis = diSlozka.GetDirectories();
					foreach (DirectoryInfo directoryInfo in dis)
					{
						ZkontrolujPosloupnostCislovaniVeSlozce(directoryInfo);
					}
				}
		}

		private void PorovnejAdresare(string strVychoziSlozka, string strCilovaSlozka) {
			DirectoryInfo di = new DirectoryInfo(strVychoziSlozka);

			FileInfo[] fis = di.GetFiles(mknsNastaveni.MaskaSouboru, mknsNastaveni.VcetnePodslozek ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			SortedDictionary<string, int> sdSoubory = new SortedDictionary<string, int>();
			foreach (FileInfo fi in fis)
			{
				sdSoubory.Add(fi.FullName.Substring(strVychoziSlozka.Length + 1), 1);
			}

			di = new DirectoryInfo(strCilovaSlozka);
			fis = di.GetFiles(mknsNastaveni.MaskaSouboru, mknsNastaveni.VcetnePodslozek ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			foreach (FileInfo fi in fis)
			{
				string sNazev = fi.FullName.Substring(strCilovaSlozka.Length);
				if (!sdSoubory.ContainsKey(sNazev))
				{
					swLog.WriteLine(fi.FullName);
				}
			}
		}

	}
}
