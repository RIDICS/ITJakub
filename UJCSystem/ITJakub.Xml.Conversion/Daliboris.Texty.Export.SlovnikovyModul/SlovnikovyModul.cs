using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Daliboris.Slovniky;
using Daliboris.Texty.Evidence.Rozhrani;
using Daliboris.Texty.Export.Rozhrani;
using Ujc.Ovj.Tools.Xml.XsltTransformation;

namespace Daliboris.Texty.Export.SlovnikovyModul
{
	public class SlovnikovyModul : ExportBase
	{
		public override void Exportuj()
		{

		}

		public SlovnikovyModul(IExportNastaveni emnNastaveni) : base(emnNastaveni)
		{
		}

		public override void Exportuj(IEnumerable<IPrepis> prpPrepisy)
		{
			ExportujImpl(prpPrepisy);
		}

		private void ExportujImpl(IEnumerable<IPrepis> prpPrepisy)
		{
			IPrepis first = (from p in prpPrepisy select p).FirstOrDefault();
			string start = first.Soubor.NazevBezPripony.Substring(0, first.Soubor.NazevBezPripony.IndexOf("_")).ToLowerInvariant();

			IList<IXsltTransformer> step01 = XsltTransformerFactory.GetXsltTransformers(Nastaveni.SouborTransformaci, (start + "-step01"), Nastaveni.SlozkaXslt);
			IList<IXsltTransformer> step02 = XsltTransformerFactory.GetXsltTransformers(Nastaveni.SouborTransformaci, (start + "-step02"), Nastaveni.SlozkaXslt);
			IList<IXsltTransformer> step03 = XsltTransformerFactory.GetXsltTransformers(Nastaveni.SouborTransformaci, (start + "-step03"), Nastaveni.SlozkaXslt);

			foreach (IPrepis prepis in prpPrepisy)
			{
				Zaloguj("Převádím soubor {0}", prepis.Soubor.Nazev, false);

				string vystupniSoubor = null;
				string konecnyVystup = null;

				DateTime casExportu = Nastaveni.CasExportu == DateTime.MinValue ? DateTime.Now : Nastaveni.CasExportu;
				string souborBezPripony = prepis.Soubor.NazevBezPripony;
				try
				{
					int step = 0;
					const string csPriponaXml = ".xml";
					konecnyVystup = Path.Combine(Nastaveni.VystupniSlozka, souborBezPripony + csPriponaXml);

					const string fileNameFormat = "{0}_{1:00}.xml";
					string step00File = Path.Combine(Nastaveni.DocasnaSlozka, String.Format(fileNameFormat, souborBezPripony, step++));
					NameValueCollection parameters = new NameValueCollection();
					ApplyTransformations(Path.Combine(Nastaveni.VstupniSlozka, souborBezPripony + csPriponaXml),
						step00File, step01, Nastaveni.DocasnaSlozka, parameters);
					vystupniSoubor = step00File;

					string step01File = Path.Combine(Nastaveni.DocasnaSlozka, String.Format(fileNameFormat, souborBezPripony, step++));
					ESSC essc = new ESSC(vystupniSoubor, step01File);
					essc.SeskupitHeslaPismene();

					string step02File = Path.Combine(Nastaveni.DocasnaSlozka, String.Format(fileNameFormat, souborBezPripony, step++));
					essc.VstupniSoubor = step01File;
					essc.VystupniSoubor = step02File;
					essc.UpravitHraniceHesloveStati();

					string step03File = Path.Combine(Nastaveni.DocasnaSlozka, String.Format(fileNameFormat, souborBezPripony, step++));
					essc.VstupniSoubor = step02File;
					essc.VystupniSoubor = step03File;
					essc.KonsolidovatHeslovouStat();

					string step04File = Path.Combine(Nastaveni.DocasnaSlozka, String.Format(fileNameFormat, souborBezPripony, step++));
					essc.VstupniSoubor = step03File;
					essc.VystupniSoubor = step04File;
					essc.UpravitOdkazy();

					string step05File = Path.Combine(Nastaveni.DocasnaSlozka, String.Format(fileNameFormat, souborBezPripony, step++));
					essc.VstupniSoubor = step04File;
					essc.VystupniSoubor = step05File;
					//essc.IdentifikovatZkratky(Nastaveni.);

					parameters = new NameValueCollection();
					ApplyTransformations(step04File, step05File, step02, Nastaveni.DocasnaSlozka, parameters);


					string step06File = Path.Combine(Nastaveni.DocasnaSlozka, String.Format(fileNameFormat, souborBezPripony, step++));
					ApplyTransformations(step05File, step06File, step03, Nastaveni.DocasnaSlozka, parameters);

					vystupniSoubor = step06File;

				}
				catch (Exception e)
				{
					Zaloguj("Při konverzi souboru {0} nastala chyba: {1}", prepis.Soubor.NazevBezPripony, e.Message, true);

				}
				finally
				{
					if (vystupniSoubor != null)
					{
						if (konecnyVystup != null && File.Exists(konecnyVystup))
							File.Delete(konecnyVystup);
						File.Copy(vystupniSoubor, konecnyVystup);
						File.SetCreationTime(konecnyVystup, casExportu);

						//if (Nastaveni.SmazatDocasneSoubory)
						//	ekup.SmazDocasneSoubory();
					}
				}
			}
		}
	}
}