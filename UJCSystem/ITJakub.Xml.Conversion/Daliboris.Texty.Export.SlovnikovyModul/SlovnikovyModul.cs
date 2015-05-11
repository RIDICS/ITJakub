using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Daliboris.Texty.Evidence.Rozhrani;
using Ujc.Ovj.Tools.Xml.XsltTransformation;

namespace Daliboris.Texty.Export.SlovnikovyModul
{
	public class SlovnikovyModul : ExportBase
	{
		public override void Exportuj()
		{

		}

		public override void Exportuj(IEnumerable<IPrepis> prpPrepisy)
		{
			ExportujImpl(prpPrepisy);
		}

		private void ExportujImpl(IEnumerable<IPrepis> prpPrepisy)
		{
			IPrepis first = (from p in prpPrepisy select p).FirstOrDefault();
			string start = first.Soubor.NazevBezPripony.Substring(0, first.Soubor.NazevBezPripony.IndexOf("_"));

			IList<IXsltTransformer> step01 = XsltTransformerFactory.GetXsltTransformers(Nastaveni.SouborTransformaci, start + "-step01", Nastaveni.SlozkaXslt);

			foreach (IPrepis prepis in prpPrepisy)
			{
				Zaloguj("Převádím soubor {0}", prepis.Soubor.Nazev, false);

				string strVystup = null;
				string sKonecnyVystup = null;

				DateTime casExportu = Nastaveni.CasExportu == DateTime.MinValue ? DateTime.Now : Nastaveni.CasExportu;
				string souborBezPripony = prepis.Soubor.NazevBezPripony;
				try
				{
					const string csPriponaXml = ".xml";
					sKonecnyVystup = Path.Combine(Nastaveni.VystupniSlozka, souborBezPripony + csPriponaXml);

					string step00File = Path.Combine(Nastaveni.DocasnaSlozka, String.Format("{0}.xml", souborBezPripony));
					NameValueCollection parameters = new NameValueCollection();
					ApplyTransformations(Path.Combine(Nastaveni.VstupniSlozka, souborBezPripony + csPriponaXml),
						step00File, step01, Nastaveni.DocasnaSlozka, parameters);

				}
				catch (Exception e)
				{
					Zaloguj("Při konverzi souboru {0} nastala chyba: {1}", prepis.Soubor.NazevBezPripony, e.Message, true);

				}
				finally
				{
					if (strVystup != null)
					{
						if (sKonecnyVystup != null && File.Exists(sKonecnyVystup))
							File.Delete(sKonecnyVystup);
						File.Copy(strVystup, sKonecnyVystup);
						File.SetCreationTime(sKonecnyVystup, casExportu);

						//if (Nastaveni.SmazatDocasneSoubory)
						//	ekup.SmazDocasneSoubory();
					}
				}
			}
		}
	}
}