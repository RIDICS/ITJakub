using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Daliboris.Texty.Evidence.Rozhrani;
using Daliboris.Texty.Export.Rozhrani;
using Ujc.Ovj.Tools.Xml.XsltTransformation;

namespace Daliboris.Texty.Export
{
	public class EdicniModul : ExportBase
	{
		const string cstrPriponaXsl = ".xsl";

		public EdicniModul() { }

		public EdicniModul(IExportNastaveni emnNastaveni) : base(emnNastaveni)
		{ }

		/*
			public void UpravBiblickyText(string strNazev, string strVstup)
			{
			 List<string> glsVystupy = new List<string>();
			 List<string> glsTransformacniSablony = new List<string>();
			 glsVystupy.Add(strVstup);
			 glsTransformacniSablony.Add(strVstup);
			 EdicniModulUpravy.ZpracovatBiblickyText(Nastaveni, strNazev, glsTransformacniSablony, glsVystupy, strVstup);

			}
	 */

		public override void Exportuj()
		{
			IPrepis prepis = Nastaveni.Prepis;

			const string sSlozkaXslt = @"D:\!UJC\OVJ\Texty\Konverze\Xslt\";
			List<string> glsXslt = new List<string>();

			glsXslt.Add(sSlozkaXslt + "Prevod_starych_wordovskych_stylu_na_nove" + ".xsl");
			glsXslt.Add(sSlozkaXslt + "Odstranit_tabulku_metadat" + ".xsl");
			glsXslt.Add(sSlozkaXslt + "Odstranit_interni_poznamky" + ".xsl");
			glsXslt.Add(sSlozkaXslt + "Prevest_sloucene_styly_na_elementy" + ".xsl");
			glsXslt.Add(sSlozkaXslt + "Sloucit_obsah_nasledujicich_shodnych_elementu" + ".xsl");
			glsXslt.Add(sSlozkaXslt + "Odstranit_mezery_mezi_elementy" + ".xsl");
			glsXslt.Add(sSlozkaXslt + "Odstranit_prazdne_elementy" + ".xsl");

			glsXslt.Add(sSlozkaXslt + "EM_Prevod_stylu_na_TEI" + ".xsl");
			glsXslt.Add(sSlozkaXslt + "Upravit_obsah_prvku_cell" + ".xsl");

			glsXslt.Add(sSlozkaXslt + "Seskupit_prvky_thead_do_div" + ".xsl");//EB-výbor
			//glsXslt.Add(sSlozkaXslt + "EB_Odstranit_edicni_komentar" + ".xsl");//EB-výbor

			//glsXslt.Add(sSlozkaXslt + "EB_Vybor_Seskupit_prvky_do_div" + ".xsl");
			glsXslt.Add(sSlozkaXslt + "Seskupit_prvky_do_div" + ".xsl"); //EB
			glsXslt.Add(sSlozkaXslt + "Seskupit_prvky_k_titulu" + ".xsl");//EB
			glsXslt.Add(sSlozkaXslt + "Presunout_foliaci_pred_odstavec" + ".xsl");//EB
			glsXslt.Add(sSlozkaXslt + "Presunout_foliaci_pred_odstavec" + ".xsl");//EB
			glsXslt.Add(sSlozkaXslt + "TEI_Upravit_Foliaci" + ".xsl");
			glsXslt.Add(sSlozkaXslt + "TEI_Seskupit_odstavce_bez_div" + ".xsl");//EB
			//glsXslt.Add(sSlozkaXslt + "EB_Vybor_Seskupit_odstavce_bez_div" + ".xsl");//EB-výbor
			//glsXslt.Add(sSlozkaXslt + "EB_Vybor_Seskupit_verse_bez_div" + ".xsl");//EB-výbor
			glsXslt.Add(sSlozkaXslt + "TEI_Slouceni_l_a_lb" + ".xsl");

			glsXslt.Add(sSlozkaXslt + "Sloucit_corr_a_sic" + ".xsl");
			glsXslt.Add(sSlozkaXslt + "Prevest_prvek_text_na_text" + ".xsl");

			glsXslt.Add(sSlozkaXslt + "Odstranit_mezery_mezi_elementy" + ".xsl");
			glsXslt.Add(sSlozkaXslt + "Prejmenovat_head1" + ".xsl");
			glsXslt.Add(sSlozkaXslt + "EM_Seskupit_polozky_rejstriku" + ".xsl");
			glsXslt.Add(sSlozkaXslt + "EM_Prevod_relatoru" + ".xsl");

			//glsXslt.Add(sSlozkaXslt + "Presunout_foliaci_pred_odstavec" + ".xsl");
			//glsXslt.Add(sSlozkaXslt + "Presunout_foliaci_pred_odstavec" + ".xsl");

			//glsXslt.Add(sSlozkaXslt + "EM_Ukazka_s_hlavickou" + ".xsl");

            
				//EdicniModulUpravy ups = new EdicniModulUpravy();
				//if (prp.LiterarniZanr != "biblický text") continue;
				//if (prp.NazevSouboru != "BiblKladrGn.docx") continue;
				//if (prp.NazevSouboru != "PN_ZrcSpasK_1-22.docx") continue;
				//if (prp.NazevSouboru != "HKMP_BiblKlemNZ_Mt.docx") continue;
				//if (prp.NazevSouboru != "CestKabK.docx") continue;
				try
				{
					EdicniModulUpravy.Uprav(prepis, Nastaveni, glsXslt);
				}
				catch (Exception e)
				{
					Zaloguj(String.Format("Chyba: {0} [{1}]", prepis.Soubor.Nazev, e.Message), true);
					//Console.WriteLine();
				}

				//prp.Zpracovani.ZaevidujExport(ZpusobVyuziti.EdicniModul, DateTime.Now);
			
		}

		public override void Exportuj(IPrepis prpPrepis)
		{
			ExportujImpl(prpPrepis);
		}

		private void ExportujImpl(IPrepis prepis)
		{
			IList<IXsltTransformer> header = XsltTransformerFactory.GetXsltTransformers(Nastaveni.SouborTransformaci, "header", Nastaveni.SlozkaXslt);
			IList<IXsltTransformer> front = XsltTransformerFactory.GetXsltTransformers(Nastaveni.SouborTransformaci, "front", Nastaveni.SlozkaXslt);
			IList<IXsltTransformer> body = XsltTransformerFactory.GetXsltTransformers(Nastaveni.SouborTransformaci, "body", Nastaveni.SlozkaXslt);
			IList<IXsltTransformer> back = XsltTransformerFactory.GetXsltTransformers(Nastaveni.SouborTransformaci, "back", Nastaveni.SlozkaXslt);
			IList<IXsltTransformer> joining = XsltTransformerFactory.GetXsltTransformers(Nastaveni.SouborTransformaci, "joining", Nastaveni.SlozkaXslt);
			IList<IXsltTransformer> afterJoining = XsltTransformerFactory.GetXsltTransformers(Nastaveni.SouborTransformaci, "afterJoining", Nastaveni.SlozkaXslt);



			IUpravy ekup = new EdicniModulUpravy(Nastaveni);
			
			ekup.NastavVychoziHodnoty();
			Zaloguj("Převádím soubor {0}", prepis.Soubor.Nazev, false);

			string strVystup = null;
			string sKonecnyVystup = null;

			DateTime casExportu = Nastaveni.CasExportu == DateTime.MinValue ? DateTime.Now : Nastaveni.CasExportu;
			string souborBezPripony = prepis.Soubor.NazevBezPripony;
			try
			{
				const string csPriponaXml = ".xml";
				sKonecnyVystup = Path.Combine(Nastaveni.VystupniSlozka, prepis.Soubor.NazevBezPripony + csPriponaXml);

				FileInfo fi = new FileInfo(sKonecnyVystup);
			    if (fi.Exists && fi.CreationTime == casExportu)
			        return;

			    string headerFile = Path.Combine(Nastaveni.DocasnaSlozka, String.Format("{0}_{1}.xml", souborBezPripony, "header"));
			    var parameters = new NameValueCollection {{"soubor", prepis.Soubor.Nazev}};
			    ApplyTransformations(Nastaveni.SouborMetadat, headerFile, header, Nastaveni.DocasnaSlozka, parameters);

				string frontFile = Path.Combine(Nastaveni.DocasnaSlozka, String.Format("{0}_{1}.xml", souborBezPripony, "front"));
				ApplyTransformations(Nastaveni.SouborMetadat, frontFile, front, Nastaveni.DocasnaSlozka, parameters);

			    parameters = new NameValueCollection{{"exportovatTransliteraci", prepis.Zpracovani.Transliterovane ? "true()" : "false()"}};
			    string bodyFile = Path.Combine(Nastaveni.DocasnaSlozka, String.Format("{0}_{1}.xml", souborBezPripony, "body"));
				ApplyTransformations(Path.Combine(Nastaveni.VstupniSlozka, souborBezPripony + csPriponaXml),
					bodyFile, body, Nastaveni.DocasnaSlozka, parameters);

				parameters.Add("hlavicka", headerFile);
				parameters.Add("zacatek", frontFile);

				string combineFile = Path.Combine(Nastaveni.DocasnaSlozka, String.Format("{0}_{1}.xml", souborBezPripony, "joining"));
				ApplyTransformations(bodyFile, combineFile, joining, Nastaveni.DocasnaSlozka, parameters);


				parameters = new NameValueCollection();
				string afterCombineFile = Path.Combine(Nastaveni.DocasnaSlozka, String.Format("{0}_{1}.xml", souborBezPripony, "afterJoining"));
				ApplyTransformations(combineFile, afterCombineFile, afterJoining, Nastaveni.DocasnaSlozka, parameters);

				List<UpravaSouboruXml> lsup = new List<UpravaSouboruXml>();

				lsup.Add(EdicniModulUpravy.PresunoutMezeryVneTagu);
				lsup.Add(EdicniModulUpravy.PridatMezeryZaTagyPoInterpunkci);
                lsup.Add(EdicniModulUpravy.RozdelitNaSlova);
				//lsup.Add(Upravy.UpravitTextTypograficky);
				EdicniModulUpravy eu = ekup as EdicniModulUpravy;
				strVystup = ekup.ProvedUpravy(prepis, afterCombineFile, lsup);
			}
			catch (Exception e)
			{
				Zaloguj("Při konverzi souboru {0} nastala chyba: {1}", prepis.Soubor.NazevBezPripony, e.Message, true);
			}
			finally
			{
				if (strVystup != null)
				{
					if (File.Exists(sKonecnyVystup))
						File.Delete(sKonecnyVystup);
					File.Copy(strVystup, sKonecnyVystup);
					File.SetCreationTime(sKonecnyVystup, casExportu);

					if (Nastaveni.SmazatDocasneSoubory)
						ekup.SmazDocasneSoubory();
				}
			}
		}

		private void ExportujImplOld(IEnumerable<IPrepis> prpPrepisy)
		{
			List<ITransformacniKrok> lsHlavicka = NactiTransformacniKrokyHlavicky();
			List<ITransformacniKrok> lsUvod = NactiTransformacniKrokyUvodu();
			List<ITransformacniKrok> lsTelo = NactiTransformacniKrokyTela();
			List<ITransformacniKrok> lsPoSpojeni = NactiTransformacniKrokyPoSpojeni();
			List<ITransformacniKrok> lsZaver = null;
			//List<ITransformacniKrok> lsPrirazeniId = NactiTransformacniKrokyPrirazeniId();

			int iPoradi = 0;
			IUpravy ekup = new EdicniModulUpravy(Nastaveni);

			foreach (IPrepis prepis in prpPrepisy)
			{
				ekup.NastavVychoziHodnoty();
				Zaloguj("Převádím soubor {0}", prepis.Soubor.Nazev, false);
				Console.WriteLine();
				Console.WriteLine("Převádím soubor {0}", prepis.Soubor.Nazev);

				iPoradi++;

				string strVystup = null;
				string sKonecnyVystup = null;

				DateTime casExportu = Nastaveni.CasExportu == DateTime.MinValue ? DateTime.Now : Nastaveni.CasExportu;

				try
				{
					const string csPriponaXml = ".xml";
					sKonecnyVystup = Path.Combine(Nastaveni.VystupniSlozka, prepis.Soubor.NazevBezPripony + csPriponaXml);

					FileInfo fi = new FileInfo(sKonecnyVystup);
					if (fi.Exists && fi.CreationTime == casExportu)
						continue;

					string strHlavicka = ekup.UpravHlavicku(prepis, lsHlavicka);
					string strUvod = ekup.UpravUvod(prepis, lsUvod);
					string strTelo = ekup.UpravTelo(prepis, lsTelo);
					string strZaver = ekup.UpravZaver(prepis, lsZaver);


					strVystup = ekup.DejNazevVystupu(prepis);

					//strVystup = Path.Combine(ekup.DocasnaSlozka, prepis.Soubor.NazevBezPripony + csPriponaXml);

					//strTelo = Vybor_UpravBiblickyText(strTelo);

					List<ITransformacniKrok> lsSpojeni = NactiTransformacniKrokySpojeni(strHlavicka, strUvod);
					strVystup = ekup.ZkombinujCastiTextu(prepis, strHlavicka, strUvod, strTelo, strZaver, lsSpojeni);

					//ZkombinujCastiTextu(strHlavicka, strUvod, strTelo, strZaver, strVystup);

					//strVystup = ekup.DejNazevVystupu(prepis);


					strVystup = ekup.UpravPoSpojeni(prepis, lsPoSpojeni);

					List<UpravaSouboruXml> lsup = new List<UpravaSouboruXml>();

					lsup.Add(EdicniModulUpravy.PresunoutMezeryVneTagu);
					lsup.Add(EdicniModulUpravy.PridatMezeryZaTagyPoInterpunkci);
					//lsup.Add(Upravy.UpravitTextTypograficky);
					EdicniModulUpravy eu = ekup as EdicniModulUpravy;
					strVystup = ekup.ProvedUpravy(prepis, strVystup, lsup);
				}
				catch (Exception e)
				{
					//Console.WriteLine("Při konverzi nastala chyba. " + e.Message);
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

						if (Nastaveni.SmazatDocasneSoubory)
							ekup.SmazDocasneSoubory();
					}
				}

				if (Nastaveni.Evidovat)
					prepis.Zpracovani.ZaevidujExport(ZpusobVyuziti.EdicniModul, casExportu, prepis.Soubor.KontrolniSoucet);
			}
		}

		private List<ITransformacniKrok> NactiTransformacniKrokySpojeni(string strHlavicka, string strUvod)
		{
			string strOznaceni = "EM_Spojit_s_hlavickou_a_front";
			List<ITransformacniKrok> lst = new List<ITransformacniKrok>();
			TransformacniKrok tk = new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl));
			tk.Parametry = new Dictionary<string, string>();
			tk.Parametry.Add("hlavicka", strHlavicka);
			tk.Parametry.Add("zacatek", strUvod);
			lst.Add(tk);
			return lst;
		}

		private List<ITransformacniKrok> NactiTransformacniKrokyPoSpojeni()
		{
			List<ITransformacniKrok> lst = new List<ITransformacniKrok>();
			//string sSoubor = Path.Combine(Nastaveni.SlozkaXslt, "EM_Presunout_edicni_komentar" + cstrPriponaXsl);
			string sSoubor = Path.Combine(Nastaveni.SlozkaXslt, "EM_Presunout_edicni_komentar_x" + cstrPriponaXsl);

			ITransformacniKrok tk = new TransformacniKrok("EM_Presunout_edicni_komentar", sSoubor);
			lst.Add(tk);

			//sSoubor = Path.Combine(Nastaveni.SlozkaXslt, "EB_Vybor_Priradit_Id" + cstrPriponaXsl);
			//tk = new TransformacniKrok("EB_Vybor_Priradit_Id", sSoubor);
			//lst.Add(tk);
			return lst;

		}

		private List<ITransformacniKrok> NactiTransformacniKrokyHlavicky()
		{
			List<ITransformacniKrok> lst = new List<ITransformacniKrok>();
			string sSoubor = Path.Combine(Nastaveni.SlozkaXslt, "EM_Vytvorit_hlavicku_TEI" + cstrPriponaXsl);
			ITransformacniKrok tk = new TransformacniKrok("EM_Vytvorit_hlavicku_TEI", sSoubor);
			tk.Parametry = new Dictionary<string, string>();
			tk.Parametry.Add("soubor", null);
			lst.Add(tk);
			return lst;
		}

		private List<ITransformacniKrok> NactiTransformacniKrokyUvodu()
		{
			List<ITransformacniKrok> lst = new List<ITransformacniKrok>();
			string sSoubor = Path.Combine(Nastaveni.SlozkaXslt, "Vytvorit_front_TEI" + cstrPriponaXsl);
			ITransformacniKrok tk = new TransformacniKrok("Vytvorit_front_TEI", sSoubor);
			tk.Parametry = new Dictionary<string, string>();
			tk.Parametry.Add("soubor", null);
			lst.Add(tk);
			return lst;
		}

		private List<ITransformacniKrok> NactiTransformacniKrokyTela()
		{
			List<ITransformacniKrok> lst = new List<ITransformacniKrok>();

			string strOznaceni = null;

			strOznaceni = "Prevod_starych_wordovskych_stylu_na_nove";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			strOznaceni = "Odstranit_tabulku_metadat";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			strOznaceni = "Odstranit_interni_poznamky";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			strOznaceni = "Prevest_sloucene_styly_na_elementy";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			strOznaceni = "Sloucit_obsah_nasledujicich_shodnych_elementu";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			strOznaceni = "Odstranit_mezery_mezi_elementy";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			strOznaceni = "Odstranit_prazdne_elementy";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			strOznaceni = "EM_Prevod_stylu_na_TEI";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			strOznaceni = "Upravit_obsah_prvku_cell";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));


			/*
		 */

			//strOznaceni = "EB_Vybor_Seskupit_prvky_do_div";
			/*
		 strOznaceni = "TEST_Seskupit_prvky_do_div";
		 lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));

		 strOznaceni = "TEST_Seskupit_prvky_thead_do_div";
		 lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			*/

			/*
			strOznaceni = "Seskupit_prvky_do_div";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			*/

			strOznaceni = "Seskupit_prvky_do_div_2.0_x";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));

			/*
		 strOznaceni = "Seskupit_prvky_do_div_2.0";
		 lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			*/

			/*
		 strOznaceni = "TEI_Seskupit_odstavce_bez_div";
		 lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			*/

			strOznaceni = "TEI_Seskupit_odstavce_bez_div_2.0_x";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));


			strOznaceni = "Seskupit_table_do_div_x";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));


			strOznaceni = "Seskupit_prvky_do_div_2.0_2_x";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));

			strOznaceni = "Seskupit_prvky_head1_do_div_2.0";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));

			strOznaceni = "Seskupit_prvky_head_do_div_2.0_x";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));

			/*
		 strOznaceni = "Seskupit_prvky_do_div_2.0";
		 lst.Add(new TransformacniKrok(strOznaceni + "_2", Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
		 */

			/*
		strOznaceni = "Seskupit_prvky_do_div1";
		lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
		*/

			//////////////strOznaceni = "Seskupit_prvky_k_titulu";
			////////////// lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));

			for (int i = 0; i < 3; i++)
			{
				strOznaceni = "Presunout_foliaci_pred_odstavec";
				//strOznaceni = "TEST_Presunout_foliaci_pred_odstavec";
				lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));

			}

			strOznaceni = "TEI_Upravit_Foliaci";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));

			/*
		 strOznaceni = "TEST_TEI_Seskupit_odstavce_bez_div";
		 lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			*/


			//////strOznaceni = "TEI_Seskupit_odstavce_bez_div";
			//////lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));



			strOznaceni = "TEST_Seskupit_table_do_div";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));

			strOznaceni = "TEI_Slouceni_l_a_lb";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));

			strOznaceni = "Sloucit_corr_a_sic";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			strOznaceni = "Prevest_prvek_text_na_text";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));

			strOznaceni = "Odstranit_mezery_mezi_elementy";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			strOznaceni = "Prejmenovat_head1";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			////strOznaceni = "EM_Seskupit_polozky_rejstriku";
			////lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));

			strOznaceni = "EM_Seskupit_polozky_rejstriku_2.0_x";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));

			strOznaceni = "EM_Prevod_relatoru";
			lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));
			return lst;
		}

		private void ZkombinujCastiTextu(string strHlavicka, string strUvod, string strTelo, string strZaver, string strVystup)
		{

			string strOznaceni = "EM_Spojit_s_hlavickou_a_front";
			//Console.WriteLine("{0} => {1}, {2}", strTelo, strVystup, strOznaceni);
			Zaloguj("{0} => {1}, {2}", strTelo, strVystup, strOznaceni, false);

			List<ITransformacniKrok> lst = new List<ITransformacniKrok>();


			TransformacniKrok tk = new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl));
			tk.Parametry = new Dictionary<string, string>();
			tk.Parametry.Add("hlavicka", strHlavicka);
			tk.Parametry.Add("zacatek", strUvod);
			tk.Transformuj(strTelo, strVystup);



			//lst.Add(new TransformacniKrok(strOznaceni, Path.Combine(Nastaveni.SlozkaXslt, strOznaceni + cstrPriponaXsl)));

		}


	}
}
