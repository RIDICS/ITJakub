
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence {
	using System;
	using System.IO;
	using System.Xml;
	using System.Xml.Xsl;
	using Uloziste;
	using System.Reflection;


	public class SpravaPrepisu {

		private string mstrSlozka;
		private string mstrMaska = "*.do*?";

		public delegate void Nacteni(object sender, NacteniPrepisuEventArgs ev);
		public event Nacteni NacteniSouboru;



		public SpravaPrepisu(string strSlozka) {
			mstrSlozka = strSlozka;
		}

		public string Slozka {
			get { return mstrSlozka; }
			set { mstrSlozka = value; }
		}

		public string MaskaSouboru {
			get { return mstrMaska; }
			set { mstrMaska = value; }

		}

		public Prepisy NactiPrepisy(bool blnJenomZaklad) {
			SouborovySystem ss = new SouborovySystem(mstrSlozka);
			ss.NacteniSouboru += new SouborovySystem.Nacteni(NacteniPrepisu);
			ss.MaskaSouboru = mstrMaska;
			// ss.NacteniSouboru -= NacteniPrepisu;
			return ss.NacistPrepisy(blnJenomZaklad);
		}

		public Prepisy NactiPrepisy() {
			return NactiPrepisy(false);
		}

		public void NajdiAktualizacePrepisu(ref Prepisy prpAktualniPrepisyXml) {
			Prepisy prpZaklad = NactiPrepisy(true);


			foreach (Prepis prp in prpZaklad) {
				if (prpAktualniPrepisyXml.ExistujeSoubor(prp.NazevSouboru)) {
					if (prpAktualniPrepisyXml[prp.NazevSouboru].KontrolniSoucet != prp.KontrolniSoucet)
					//if (prpAktualniPrepisyXml[prp.NazevSouboru].Zmeneno != prp.Zmeneno)
						prpAktualniPrepisyXml[prp.NazevSouboru].Status = StatusAktualizace.Zmeneno;
					else if (prpAktualniPrepisyXml[prp.NazevSouboru].Status == StatusAktualizace.Odstraneno)
						prpAktualniPrepisyXml[prp.NazevSouboru].Status = StatusAktualizace.BezeZmen;
				}
				else {
					prp.Status = StatusAktualizace.Nove;
					prpAktualniPrepisyXml.Add(prp);
				}
			}
			
			foreach (Prepis prp in prpAktualniPrepisyXml) {
				if (!prpZaklad.ExistujeSoubor(prp.NazevSouboru))
					prp.Status = StatusAktualizace.Odstraneno;
			}
		}

		//public Prepisy NajdiNovePrepisy(Prepisy prpAktualniPrepisy) {
		//   Prepisy prpZaklad = NactiPrepisy(true);
		//   return prpZaklad;
		//}
		//public Prepisy NajdiZmenenePrepisy(Prepisy prpAktualniPrepisy) {
		//   Prepisy prpZaklad = NactiPrepisy(true);
		//   return prpZaklad;
		//}

		public bool UlozPrepis(Prepis prpPrepis) {
			SouborovySystem ss = new SouborovySystem(mstrSlozka);
			ss.NacteniSouboru += new SouborovySystem.Nacteni(NacteniPrepisu);
			return (ss.UlozitPrepis(prpPrepis));
		}

		public void NacteniPrepisu(object sender, NacteniPrepisuEventArgs ev) {
			if (NacteniSouboru != null)
				NacteniSouboru(sender, ev);
		}


		public static string Prepis2HTML(IPrepis prpPrepis) {
			//Prepis2Xml(prpPrepis);
						MemoryStream msmXml = null;
						XslCompiledTransform xctXslt = new XslCompiledTransform();
						Assembly assembly = Assembly.GetExecutingAssembly();
						//xctXslt.Load(@"V:\Projekty\Daliboris\Texty\Evidence\Xslt\Prepisy2HTML.xsl");
						FileInfo fi = new FileInfo(assembly.Location);
						xctXslt.Load(fi.DirectoryName + "\\Xslt\\Prepisy2HTML.xsl");

						if (prpPrepis.PuvodniPodoba != null)
						{
								msmXml = PorovnatPrepisy2HTML(prpPrepis, prpPrepis.PuvodniPodoba);
						}
						else
						{
								msmXml = new MemoryStream();
								Perzistence.UlozitDoStreamu(msmXml, prpPrepis as Prepis);
								//Prepis.Serialize(msmXml, prpPrepis);
						}

			msmXml.Seek(0, SeekOrigin.Begin);
			XmlReader xmrReader = XmlReader.Create(msmXml);
			MemoryStream msmXmlWriter = new MemoryStream();
			XmlWriter xmwWriter = XmlWriter.Create(msmXmlWriter);
			xctXslt.Transform(xmrReader, xmwWriter);

			byte[] byteArray = new byte[msmXmlWriter.Length];
			byteArray = msmXmlWriter.ToArray();
			//msmXmlWriter.Seek(0, SeekOrigin.Begin);
			//msmXmlWriter.Read(byteArray, 0, byteArray.Length);

			//System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
			String s = System.Text.Encoding.UTF8.GetString(byteArray);
			msmXml.Close();
			msmXmlWriter.Close();
			return s;

		}
				
				

				private static MemoryStream PorovnatPrepisy2HTML(IPrepis prpLevy, IPrepis prpPravy)
				{
						XmlDocument xdPrepisy = new XmlDocument();
						XmlDocument xdp = Prepis2Xml(prpPravy);
						XmlNode xnd = xdPrepisy.CreateElement("Prepisy");
						xnd.AppendChild(xdPrepisy.ImportNode(xdp.DocumentElement, true));
						xdp = Prepis2Xml(prpLevy);
						xnd.AppendChild(xdPrepisy.ImportNode(xdp.DocumentElement, true));
						xdPrepisy.AppendChild(xnd);

						XslCompiledTransform xctXslt = new XslCompiledTransform();
						Assembly assembly = Assembly.GetExecutingAssembly();
						FileInfo fi = new FileInfo(assembly.Location);
						xctXslt.Load(fi.DirectoryName + "\\Xslt\\PrepisySrovnani.xsl");
						MemoryStream msmXmlWriter = new MemoryStream();
						XmlWriter xmwWriter = XmlWriter.Create(msmXmlWriter);
						xctXslt.Transform(xdPrepisy.CreateNavigator(), xmwWriter);

						return msmXmlWriter;
						/*
						byte[] byteArray = new byte[msmXmlWriter.Length];
						byteArray = msmXmlWriter.ToArray();
						String s = System.Text.Encoding.UTF8.GetString(byteArray);
						msmXmlWriter.Close();
						return s;
						*/

				}

		public static XmlDocument Prepis2Xml(IPrepis prp) {
						XmlDocument xd = new XmlDocument();

			//Stream msm = new MemoryStream();
			MemoryStream msm = new MemoryStream();
			//Prepis.Serialize(msm, prp, Prepisy.csDefaultNamespace);
						Perzistence.UlozitDoStreamu(msm, prp as Prepis);
						//Prepis.Serialize(msm, prp);
			byte[] byteArr = new byte[msm.Length];
			System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
			msm.Seek(0, SeekOrigin.Begin);
			byteArr = msm.ToArray();
			//msm.Write(byteArr, 0, byteArr.Length);
			String s2 = enc.GetString(byteArr);
			//FileStream fl = File.Create(@"V:\Temp\Text_01.xml");
			//fl.Write(byteArr, 0, byteArr.Length);
			//fl.Close();
						xd.LoadXml(s2);
			msm.Close();
						return xd;
		}
	}
	/*
	public class SpravaZaznamu {
		private string mstrSouborDB;
		public SpravaZaznamu(string strSouborDB) {
			mstrSouborDB = strSouborDB;
		}
		public bool UlozPrepis(Prepis prpPrepis) {
			Databaze db = new Databaze(mstrSouborDB);
			return (db.UlozitPrepis(prpPrepis));
		}
	}
	*/
}
