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

		    var underscorePosition = first.Soubor.NazevBezPripony.IndexOf("_", StringComparison.Ordinal);
		    var fileName = first.Soubor.NazevBezPripony;
		    if (underscorePosition > 0)
		    {
		        fileName = fileName.Substring(0, underscorePosition);
		    }

            fileName = fileName.ToLowerInvariant();

            var xsltSteps = new Queue<IList<IXsltTransformer>>();
            
            foreach (var transformationFile in XsltTransformerFactory.GetTransformationFromTransformationsFile(Nastaveni.SouborTransformaci, String.Format("{0}-step", fileName)))
		    {
                xsltSteps.Enqueue(
		            XsltTransformerFactory.GetXsltTransformers(
		                Nastaveni.SouborTransformaci,
                        transformationFile,
		                Nastaveni.SlozkaXslt, true));
		    }
            
			foreach (IPrepis prepis in prpPrepisy)
			{
				Zaloguj("Převádím soubor {0}", prepis.Soubor.Nazev, false);

				string vystupniSoubor = null;
				

				var casExportu = Nastaveni.CasExportu == DateTime.MinValue ? DateTime.Now : Nastaveni.CasExportu;
				var souborBezPripony = prepis.Soubor.NazevBezPripony;
			    var fullFileName = souborBezPripony + ".xml";

                var konecnyVystup = Path.Combine(Nastaveni.VystupniSlozka, fullFileName);

                try
				{
					var step = 0;
                    
				    var step00File = GetTempFile(Nastaveni.DocasnaSlozka, souborBezPripony, step++);
					var parameters = new NameValueCollection();
					ApplyTransformations(Path.Combine(Nastaveni.VstupniSlozka, fullFileName), step00File, xsltSteps.Dequeue(), Nastaveni.DocasnaSlozka, parameters);

					vystupniSoubor = step00File;

					var slovnik = GetDictionaryObject(fileName);
                    
					var step01File = GetTempFile(Nastaveni.DocasnaSlozka, souborBezPripony, step++);
					var step02File = GetTempFile(Nastaveni.DocasnaSlozka, souborBezPripony, step++);
                    var step03File = GetTempFile(Nastaveni.DocasnaSlozka, souborBezPripony, step++);
                    var step04File = GetTempFile(Nastaveni.DocasnaSlozka, souborBezPripony, step++);

                    slovnik.SeskupitHeslaPismene(step00File, step01File);
                    slovnik.UpravitHraniceHesloveStati(step01File, step02File);
                    slovnik.KonsolidovatHeslovouStat(step02File, step03File);
                    slovnik.UpravitOdkazy(step03File, step04File);
                    
				    var fileTransformationSource = step04File;

				    parameters = new NameValueCollection();
				    while (xsltSteps.Count > 0)
				    {
                        var fileTransformationTarget = GetTempFile(Nastaveni.DocasnaSlozka, souborBezPripony, step++);
                        
                        ApplyTransformations(fileTransformationSource, fileTransformationTarget, xsltSteps.Dequeue(), Nastaveni.DocasnaSlozka, parameters);

                        fileTransformationSource = fileTransformationTarget;
				    }

					vystupniSoubor = fileTransformationSource;
				}
				catch (Exception e)
				{
					Zaloguj("Při konverzi souboru {0} nastala chyba: {1}", prepis.Soubor.NazevBezPripony, e.Message, true);
				}
				finally
				{
					if (vystupniSoubor != null)
					{
						if (File.Exists(konecnyVystup))
							File.Delete(konecnyVystup);
						File.Copy(vystupniSoubor, konecnyVystup);
						File.SetCreationTime(konecnyVystup, casExportu);

						//if (Nastaveni.SmazatDocasneSoubory)
						//	ekup.SmazDocasneSoubory();
					}
				}
			}
		}

	    protected string GetTempFile(string tempDirectory, string sourceFile, int step)
	    {
            const string fileNameFormat = "{0}_{1:00}.xml";

            return Path.Combine(tempDirectory, String.Format(fileNameFormat, sourceFile, step++));
        }

		Slovnik GetDictionaryObject(string dictionaryAcronym)
		{
			Slovnik slovnik = null;
			switch (dictionaryAcronym)
			{
				case "stcs":
					slovnik = new StcS();
				    break;

				case "essc":
					slovnik = new ESSC();
					break;

                case "gbslov":
					slovnik = new GbSlov();
					break;

                case "mss":
                    slovnik = new MSS();
                    break;
            }

			return slovnik;
		}
	}
}