
namespace Daliboris.Pomucky.Soubory {
	using System.Collections.Generic;
	using DSOFile;
	using System.IO;
	public static class Metadata {

		public enum MtdVlastnosti {
			mtdAutor,
			mtdKategorie,
			mtdTitul,
			mtdPredmet,
			mtdKomentar
		}

		public enum TypVlastnosti {
			dsoPropertyTypeUnknown = 0,
			dsoPropertyTypeString = 1,
			dsoPropertyTypeLong = 2,
			dsoPropertyTypeDouble = 3,
			dsoPropertyTypeBool = 4,
			dsoPropertyTypeDate = 5,
		}


		public static bool UlozUzivatelskeVlastnosti(string sSoubor, Dictionary<string, object> gdcVlastnosti) {
			DSOFile.OleDocumentPropertiesClass oleDocument = new DSOFile.OleDocumentPropertiesClass();
			DSOFile.CustomProperties customProperties;

			bool bUspech = true;
			object oHodnota = null;

			try {
				oleDocument.Open(sSoubor, false, dsoFileOpenOptions.dsoOptionOpenReadOnlyIfNoWriteAccess);
				customProperties = oleDocument.CustomProperties;
				foreach (KeyValuePair<string, object> kvp in gdcVlastnosti) {
					bool bNastaveno = false;
					oHodnota = kvp.Value;
					for (int i = 0; i < customProperties.Count; i++) {
						object oIndex = i;
						if (customProperties[oIndex].Name == kvp.Key) {
							customProperties[oIndex].set_Value(ref oHodnota);
							bNastaveno = true;
							break;
						}

						//foreach (DSOFile.CustomProperty prp in customProperties) {
						//if (prp.Name == kvp.Key) {
						//   prp.set_Value(ref oHodnota);
						//   bNastaveno = true;
						//   break;
						//}
					}
					if (!bNastaveno) {
						customProperties.Add(kvp.Key, ref oHodnota);
						//customProperties[customProperties.Count - 1].set_Value(ref oHodnota);
					}

				}
			}
			catch { bUspech = false; }
			finally {
				if (oleDocument != null) {
					if (oleDocument.IsDirty) {
						try {
							//v případě, že je dokument otevřen ve Wordu
							oleDocument.Close(bUspech); //v případě neúspěchu neukládát žádnou změnu
						}
						catch { bUspech = false; }
						finally { oleDocument.Close(false); }
					}
					else
						oleDocument.Close(false);
				}
			}
			return bUspech;

		}

		public static bool UlozUzivatelskouVlasnost(string sSoubor, string sNazevVlastnosti,
			dsoFilePropertyType pTypVlasnosti, object oHodnota) {
			Dictionary<string, object> gdc = new Dictionary<string, object>(1);
			gdc.Add(sNazevVlastnosti, oHodnota);
			return UlozUzivatelskeVlastnosti(sSoubor, gdc);
			////DSOFile.OleDocumentPropertiesClass oleDocument = new DSOFile.OleDocumentPropertiesClass();
			////DSOFile.CustomProperties customProperties;
			////bool bUspech = true;
			////bool bNastaveno = false;
			////try {
			////   oleDocument.Open(sSoubor, true, dsoFileOpenOptions.dsoOptionDefault);
			////   customProperties = oleDocument.CustomProperties;

			////   foreach (DSOFile.CustomProperty prp in customProperties) {
			////      if (prp.Name == sNazevVlastnosti) {
			////         prp.set_Value(ref oHodnota);
			////         bNastaveno = true;
			////         break;
			////      }
			////   }
			////   if (!bNastaveno) {
			////      customProperties.Add(sNazevVlastnosti, ref oHodnota);
			////      //customProperties[customProperties.Count - 1].set_Value(ref oHodnota);
			////   }

			////}
			////catch { bUspech = false; }
			////finally {
			////   if (oleDocument != null) {
			////      if (oleDocument.IsDirty) {
			////         oleDocument.Close(true);
			////      }
			////      else
			////         oleDocument.Close(false);
			////   }
			////}
			////return bUspech;

		}

		public static bool UlozZabudovanouVlastnost(string sSoubor, string sNazevVlasnosti, dsoFilePropertyType pTypVlasnosti, object oHodnota) {
			DSOFile.OleDocumentPropertiesClass oleDocument = new DSOFile.OleDocumentPropertiesClass();
			DSOFile.SummaryProperties summaryProperties;
			bool bUspech = true;

			try {
				oleDocument.Open(sSoubor, true, dsoFileOpenOptions.dsoOptionDefault);

				summaryProperties = oleDocument.SummaryProperties;
				//customProperties = oleDocument.CustomProperties;

				switch (sNazevVlasnosti) {
					case "Autor":
						summaryProperties.Author = oHodnota.ToString();
						break;
					case "Kategorie":
						summaryProperties.Category = oHodnota.ToString();

						break;
					case "Titul":
						summaryProperties.Title = oHodnota.ToString();
						break;
					case "Předmět":
					case "Knihovna":
						summaryProperties.Subject = oHodnota.ToString();
						break;
					case "Komentář":
						summaryProperties.Comments = oHodnota.ToString();
						break;
					default:
						bUspech = false;
						break;
				}

			}
			catch { bUspech = false; }
			finally {
				if (oleDocument != null) {
					if (oleDocument.IsDirty) {
						oleDocument.Close(true);
					}
					else
						oleDocument.Close(false);
				}
			}
			return bUspech;
		}

		public static object NactiZabudovanouVlastnost(string sSoubor, string sNazevVlasnosti) {
			DSOFile.OleDocumentPropertiesClass oleDocument = new DSOFile.OleDocumentPropertiesClass();
			DSOFile.SummaryProperties summaryProperties;
			//DSOFile.CustomProperties customProperties;
			object oHodnota = null;
			try {
				oleDocument.Open(sSoubor, true, dsoFileOpenOptions.dsoOptionDefault);

				summaryProperties = oleDocument.SummaryProperties;
				//customProperties = oleDocument.CustomProperties;

				switch (sNazevVlasnosti) {
					case "Autor":
						oHodnota = summaryProperties.Author;
						break;
					case "Kategorie":
						oHodnota = summaryProperties.Category;
						break;
					case "Titul":
						oHodnota = summaryProperties.Title;
						break;
					case "Předmět":
					case "Knihovna":
						oHodnota = summaryProperties.Subject;
						break;
					case "Komentář":
						oHodnota = summaryProperties.Comments;
						break;
					default:
						break;
				}

			}
			catch { }
			finally {
				if (oleDocument != null) {
					if (oleDocument.IsDirty)
						oleDocument.Close(true);

					else
						oleDocument.Close(false);
				}
			}
			return oHodnota;
		}

		public static object[] NactiZabudovaneVlastnosti(string sSoubor, string[] asVlastnosti) {
			DSOFile.OleDocumentPropertiesClass oleDocument = new DSOFile.OleDocumentPropertiesClass();
			DSOFile.SummaryProperties summaryProperties;
			//DSOFile.CustomProperties customProperties;
			object[] aoHodnoty = new object[asVlastnosti.Length];
			try {
				oleDocument.Open(sSoubor, true, dsoFileOpenOptions.dsoOptionDefault);

				summaryProperties = oleDocument.SummaryProperties;
				//customProperties = oleDocument.CustomProperties;
				for (int i = 0; i < asVlastnosti.Length; i++) {


					switch (asVlastnosti[i]) {
						case "Autor":
							aoHodnoty[i] = summaryProperties.Author;
							break;
						case "Kategorie":
							aoHodnoty[i] = summaryProperties.Category;
							break;
						case "Titul":
							aoHodnoty[i] = summaryProperties.Title;
							break;
						case "Předmět":
						case "Knihovna":
							aoHodnoty[i] = summaryProperties.Subject;
							break;
						case "Komentář":
							aoHodnoty[i] = summaryProperties.Comments;
							break;
						default:
							break;
					}
				}
			}
			catch { }
			finally {
				if (oleDocument != null) {
					if (oleDocument.IsDirty)
						oleDocument.Close(true);

					else
						oleDocument.Close(false);
				}
			}
			return aoHodnoty;
		}

		public static object[] NactiUzivatelskeVlastnosti(string sSoubor, string[] asNazvyVlastnosti) {
			DSOFile.OleDocumentPropertiesClass oleDocument = new DSOFile.OleDocumentPropertiesClass();
			//DSOFile.SummaryProperties summaryProperties;
			DSOFile.CustomProperties customProperties;
			object[] aoHodnoty = new object[asNazvyVlastnosti.Length];

			try {
				oleDocument.Open(sSoubor, true, dsoFileOpenOptions.dsoOptionDefault);

				//summaryProperties = oleDocument.SummaryProperties;
				customProperties = oleDocument.CustomProperties;
				foreach (CustomProperty cp in customProperties) {
					for (int i = 0; i < asNazvyVlastnosti.Length; i++) {
						if (cp.Name == asNazvyVlastnosti[i])
							aoHodnoty[i] = cp.get_Value();
					}

				}
			}
			catch { }
			finally {
				if (oleDocument != null)
					oleDocument.Close(false);
			}
			return aoHodnoty;
		}


		public static object[] NactiUzivatelskeVlastnosti(string sAdresar, string sNazevVlastnosti) {
			return NactiUzivatelskeVlastnosti(sAdresar, "*.*", sNazevVlastnosti);
		}

		public static object[] NactiUzivatelskeVlastnosti(string sAdresar, string sMaska, string sNazevVlastnosti) {
			object[] oHodnoty;
			int i = 0;
			DirectoryInfo di = new DirectoryInfo(sAdresar);
			oHodnoty = new object[di.GetFiles(sMaska).Length];
			foreach (FileInfo fi in di.GetFiles(sMaska)) {
				oHodnoty[i++] = NactiUzivatelskouVlastnost(fi.FullName, sNazevVlastnosti);
			}
			return oHodnoty;
		}

		public static object[] NactiUzivatelskeVlastnosti(string[] sSoubory, string sNazevVlastnosti) {
			object[] oHodnoty = new object[sSoubory.Length];
			for (int i = 0; i < sSoubory.Length; i++) {
				oHodnoty[i] = NactiUzivatelskouVlastnost(sSoubory[i], sNazevVlastnosti);
			}

			return oHodnoty;
		}

		public static object NactiUzivatelskouVlastnost(string sSoubor, string sNazevVlastnosti) {
			DSOFile.OleDocumentPropertiesClass oleDocument = new DSOFile.OleDocumentPropertiesClass();
			//DSOFile.SummaryProperties summaryProperties;
			DSOFile.CustomProperties customProperties;
			object oHodnota = null;

			try {
				oleDocument.Open(sSoubor, true, dsoFileOpenOptions.dsoOptionDefault);

				//summaryProperties = oleDocument.SummaryProperties;
				customProperties = oleDocument.CustomProperties;
				foreach (CustomProperty cp in customProperties) {
					if (cp.Name == sNazevVlastnosti)
						oHodnota = cp.get_Value();
				}
			}
			catch { }
			finally {
				if (oleDocument != null) {
					if (oleDocument.IsDirty)
						oleDocument.Close(true);

					else
						oleDocument.Close(false);
				}
			}
			return oHodnota;
		}

		private static CustomProperty UzivatelskaVlastnost(string sSoubor, string sNazevVlastnosti) {
			DSOFile.OleDocumentPropertiesClass oleDocument = new DSOFile.OleDocumentPropertiesClass();
			DSOFile.SummaryProperties summaryProperties;
			DSOFile.CustomProperties customProperties;
			CustomProperty cpp = null;

			try {
				oleDocument.Open(sSoubor, true, dsoFileOpenOptions.dsoOptionOpenReadOnlyIfNoWriteAccess);

				summaryProperties = oleDocument.SummaryProperties;
				customProperties = oleDocument.CustomProperties;
				foreach (CustomProperty cp in customProperties) {
					if (cp.Name == sNazevVlastnosti) {
						cpp = cp;
						break;
					}
				}
			}
			catch { }
			finally {
				if (oleDocument != null) {
					if (oleDocument.IsDirty)
						oleDocument.Close(true);

					else
						oleDocument.Close(false);
				}
			}
			return (cpp);


		}

	}
}
