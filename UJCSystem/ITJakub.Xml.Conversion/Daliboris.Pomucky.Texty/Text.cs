using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;


namespace Daliboris.Pomucky.Texty
{
		public struct Pozice {
				public string Soubor;
				public int Radek;
				public int Sloupec;
		}

	 public class Text
	 {
			public static string Retrograd(string sText, bool bVypustitZavorky)
			{
				 StringBuilder sb = new StringBuilder(sText.Length);
				 
						for (int i = sText.Length - 1; i >= 0; i--)
						{
							 switch (sText[i])
							 {
									case ')':
										 if(!bVypustitZavorky)
												sb.Append('(');
										 break;
									case '(':
										 if(!bVypustitZavorky)
												sb.Append(')');
										 break;
									default:
										 sb.Append(sText[i]);
										 break;
							 }

						}
				 if (sText.ToLower().Contains("ch"))
				 {
						sb.Replace("hc", "ch");
						sb.Replace("hC", "Ch");
						sb.Replace("Hc", "cH");
						sb.Replace("HC", "CH");
				 }


				 return sb.ToString();
			}
			public static string Retrograd(string sText)
			{
				 return Retrograd(sText, false);
			}
			public static string Retrograd(string sText, CultureInfo ciJazyk)
			{
				 return Retrograd(sText);
			}
			public static SortedDictionary<char, int> FrekvenceZnaku(string sText)
			{
					SortedDictionary<char, int> sdfZnaky = new SortedDictionary<char, int>();
					FrekvenceZnaku(ref sdfZnaky, sText);
					//for (int i = 0; i < sText.Length; i++)
					//{
					//    char ch = sText[i];
					//    if (sdfZnaky.ContainsKey(ch))
					//        sdfZnaky[ch] += 1;
									
					//    else
					//        sdfZnaky.Add(ch, 1);
					//}
					return sdfZnaky;
			}

			private static void FrekvenceZnaku(ref SortedDictionary<char, int> sdfZnaky, string sText)
			{
					for (int i = 0; i < sText.Length; i++)
					{
							char ch = sText[i];
							if (sdfZnaky.ContainsKey(ch))
									sdfZnaky[ch] += 1;

							else
									sdfZnaky.Add(ch, 1);
					}
			}

			public static SortedDictionary<char, int> FrekvenceZnaku(string sAdresar, string sMaska)
			{
					SortedDictionary<char, int> sdfZnaky = new SortedDictionary<char, int>();
					DirectoryInfo di = new DirectoryInfo(sAdresar);
					foreach (FileInfo fi in di.GetFiles(sMaska))
					{
							ZjistitZnakyVSouboru(ref sdfZnaky, fi.FullName);
					}
					return sdfZnaky;
			}

			 private static void ZjistitZnakyVSouboru(ref SortedDictionary<char, int> sdfZnaky, 
						string sSoubor)
			 {
					 StreamReader sr = File.OpenText(sSoubor);
					 string sRadek;
					 while ((sRadek = sr.ReadLine()) != null)
					 {
							 if (sRadek.IndexOf("<") == -1 )
							 {
									 FrekvenceZnaku(ref sdfZnaky, sRadek);
							 }
					 }
					 sr.Close();
					 sr = null;
			 }

			public static string NajitVyskytyZnaku(char chZnak, string sAdresar, string sMaska)
			{
					string sVyskyty = null;
					
					DirectoryInfo di = new DirectoryInfo(sAdresar);
					foreach (FileInfo fi in di.GetFiles(sMaska))
					{
							string sVyskytyVSouboru = NajdiVyskytZnakuVSouboru(chZnak, fi.FullName);
							if (sVyskytyVSouboru != null)
							{
									sVyskyty += fi.FullName + "\r\n" + sVyskytyVSouboru;
							}
					}
					return sVyskyty;
			}

			private static string NajdiVyskytZnakuVSouboru(char chZnak, string sSoubor)
			{
					string sVyskyt = null;;
					int iRadek= 0;
					StreamReader sr = File.OpenText(sSoubor);
					string sRadek;
					while ((sRadek = sr.ReadLine()) != null)
					{
							iRadek++;
							if (sRadek.IndexOf(chZnak) > -1)
							{
									sVyskyt += "radek: " + iRadek.ToString() + ", sloupec: " + sRadek.IndexOf(chZnak) + "\r\n";
							}
					}
					sr.Close();
					sr = null;
					return(sVyskyt);
			}
			public static SortedDictionary<string, int> FrekvenceRadku(string sAdresar, string sMaska)
			{
					SortedDictionary<string, int> sdfRadky = new SortedDictionary<string, int>();
					DirectoryInfo di = new DirectoryInfo(sAdresar);
					foreach (FileInfo fi in di.GetFiles(sMaska))
					{
							ZjistitRadkyVSouboru(ref sdfRadky, fi.FullName);
					}
					return sdfRadky;
			}

			private static void ZjistitRadkyVSouboru(ref SortedDictionary<string, int> sdfRadky, string sSoubor)
			{
					StreamReader sr = File.OpenText(sSoubor);
					string sRadek;
					while ((sRadek = sr.ReadLine()) != null)
					{
							if (sdfRadky.ContainsKey(sRadek))
									sdfRadky[sRadek] += 1;
							else
									sdfRadky.Add(sRadek, 1);              
					}
					sr.Close();
					sr = null;
			}

			public static void VypsatZnackuHTML(string sSouborHTML, string sZnacka, string sVystupniSoubor) {
					StreamWriter sw = new StreamWriter(sVystupniSoubor, false, Encoding.UTF8);
					using (StreamReader sr = File.OpenText(sSouborHTML))
					{
							char[] chZnak;
							char[] chZnacka = null;

							while (sr.Peek() >= 0) 
							{
									chZnak = new char[1];
									chZnacka = new char[sZnacka.Length];

									sr.Read(chZnak, 0, chZnak.Length);
									if (chZnak[0] == '<')
									{
											sr.Read(chZnacka, 0, chZnacka.Length);
											if (String.Compare(new string(chZnacka), sZnacka,true) == 0)
											{
													sw.Write(chZnak);
													sw.Write(chZnacka);
													sr.Read(chZnak, 0, chZnak.Length);
													while (chZnak[0] != '<')
													{
															sw.Write(chZnak);
															sr.Read(chZnak, 0, chZnak.Length);
													}
													sw.Write(chZnak);
													sr.Read(chZnak, 0, chZnak.Length);
													sw.Write(chZnak);
													sr.Read(chZnacka, 0, chZnacka.Length);
													sw.Write(chZnacka);
													sr.Read(chZnak, 0, chZnak.Length);
													sw.Write(chZnak);
													sw.WriteLine();
											}
											
									}
							}
							sw.Close();
							sw = null;
					}
			}

			public static void NajidJedinecne(string strPorovnatCo, string[] astrPorovnatSCim, string sVystupniSoubor) {
				List<string> mlsCo = new List<string>();
				List<string> mlsSCim = new List<string>();

				foreach (string sSoubor in astrPorovnatSCim) {
					using (StreamReader sr = new StreamReader(sSoubor)) {
						string sRadek;
						while ((sRadek = sr.ReadLine()) != null) {
							if(!mlsSCim.Contains(sRadek))
								mlsSCim.Add(sRadek);
						}
					}
				}

				using (StreamReader sr = new StreamReader(strPorovnatCo)) {
					string sRadek;
					while ((sRadek = sr.ReadLine()) != null) {
						if (!mlsSCim.Contains(sRadek))
							mlsCo.Add(sRadek);
					}
				}

				using (StreamWriter sw = new StreamWriter(sVystupniSoubor, false, Encoding.Unicode)) {
					foreach (string s in mlsCo) {
						sw.WriteLine(s);
					}
				}

			}
	 }



}
