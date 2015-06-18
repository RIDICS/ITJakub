using System;
using System.Globalization;
using System.Text;

namespace Daliboris.Pomucky.Funkce.Textove
{
  public static class Retezce
  {
	/// <summary>
	/// Vrátí poslední znak ze zadaného textu
	/// </summary>
	/// <param name="sText"></param>
	/// <returns></returns>
	public static char PosledniZnak(string sText)
	{
	  if (String.IsNullOrEmpty(sText))
		return (new char());
	  return (sText[sText.Length - 1]);
	}

	/// <summary>
	/// Vrátí první znak ze zadaného textu
	/// </summary>
	/// <param name="sText"></param>
	/// <returns></returns>
	public static char PrvniZnak(string sText)
	{
	  if (String.IsNullOrEmpty(sText))
		return (new char());
	  return (sText[0]);
	}

	/// <summary>
	/// Zjišťuje, jestli text obsahuje závorky uprostřed slova
	/// </summary>
	/// <param name="sText">Text, u něhož se zjišŤuje přítomnost závorek</param>
	/// <param name="sZavorky">Párové závorky předané jako text; nejprve počáteční, pak koncová</param>
	/// <returns></returns>
	public static bool ObsahujeZavorkyUprostredSlova(string sText, string sZavorky)
	{
	  if (String.IsNullOrEmpty(sText))
		return false;
	  if ((sText.Contains(sZavorky[0].ToString())) & !((PrvniZnak(sText) == sZavorky[0]) & (PosledniZnak(sText) == (sZavorky[1]))))
		//pro případ, že text obsahuje jenom jednu závorku
		if (sText.IndexOf(sZavorky[0]) == -1 | sText.IndexOf(sZavorky[1]) == -1)
		  return false;
		else
		  return true;
	  return false;
	}


	/// <summary>
	/// Odstraní diakritická znaménka písmen z textu
	/// </summary>
	/// <param name="sText">Text, z něhož se odstraní diakritická znaménka</param>
	/// <returns>Vrací text s písmeny bez diakritických znamének</returns>
	public static string OdstranitDiakritiku(string sText)
	{
	  String normalizedString = sText.Normalize(NormalizationForm.FormD);
	  StringBuilder stringBuilder = new StringBuilder();

	  for (int i = 0; i < normalizedString.Length; i++)
	  {
		Char c = normalizedString[i];
		if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
		  stringBuilder.Append(c);
	  }

	  return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
	}

	/// <summary>
	/// Převede řetězec na znaky ASCII.
	/// </summary>
	/// <param name="sText">Text, který se má převést na znaky ASCII</param>
	/// <param name="chNahradniZnak">Znak ASCII, který nahradí nepodporované znaky ve vstupním textu</param>
	/// <returns>Vrací řetězec obsahující pouze znaky ASCII</returns>
	public static string PrevestNaASCII(string sText, char chNahradniZnak)
	{
	  for (int i = 0; i < sText.Length; i++)
	  {
		if (sText[i] < 33 || sText[i] > 127)
		  sText = sText.Replace(sText[i], chNahradniZnak);
	  }
	  return sText;
	}

	/// <summary>
	/// Určuje, zda řetězec obsahuje pouze znaky ASCII
	/// </summary>
	/// <param name="sText">Text, který se kontroluje na přítomnot znaků ASCII</param>
	/// <returns>Vrací True, pokud řetězec obsahuje pouze znaky ASCII, False, pokud řetězec obsahuje alespoň jeden znak mimo tabulku ASCII.</returns>
	public static bool JeASCII(string sText)
	{
	  foreach (char ch in sText)
	  {
		if (Znaky.JeASCII(ch))
		  return false;
	  }
	  return true;
	}

	public static string Reverze(string sText, bool bZachovatSprezky)
	{
	  StringBuilder sb = new StringBuilder(sText.Length);

	  if (sText.ToLower().Contains("ch") & bZachovatSprezky)
	  {
		for (int i = sText.Length - 1; i >= 0; i--)
		{
		  if (i > 0 && sText[i] == 'h' | sText[i] == 'H')
		  {
			if (sText[i - 1] == 'c' | sText[i - 1] == 'C')
			{
			  sb.Append(sText[i - 1]);
			  sb.Append(sText[i]);
			  i--;
			}
			else
			  sb.Append(sText[i]);
		  }
		  else
			sb.Append(sText[i]);
		}
	  }
	  else
	  {
		//zkontrolovat ch, závorky ve slovech
		for (int i = sText.Length - 1; i >= 0; i--)
		{
		  sb.Append(sText[i]);
		}
	  }
	  return sb.ToString();
	}

	public static string Reverze(string sText)
	{
	  return Reverze(sText, false);
	}

	public static int PocetShodnychZnaku(string sText, string sSCim)
	{
	  int iPocet = 0;
	  for (int i = 0; i < (sText.Length < sSCim.Length ? sText.Length : sSCim.Length); i++)
	  {
		if (sText[i] == sSCim[i])
		  iPocet = i + 1;
		else
		  break;
	  }
	  return iPocet;
	}

	/// <summary>
	/// Připojí k textu interpunkci včetně mezery, pokud následující text není prázdný
	/// </summary>
	/// <param name="sText">Text, k němuž se připojuje interpunkce</param>
	/// <param name="sInterpunkce">Interpunkční znaménko připojované za text</param>
	/// <param name="sNasledujiciText">Text, který bude následovat za aktuálním textem</param>
	/// <returns>Pokud následující text není prázdný, vrátí text s interpunkcí a mezerou, v opačném případě vrátí text</returns>
	public static string PropojitInterpunkci(string sText, string sInterpunkce, string sNasledujiciText)
	{
	  if (sInterpunkce == null)
		throw new NullReferenceException("Parametr sInterpunkce nemůže být null");
	  if (String.IsNullOrEmpty(sNasledujiciText))
		return sText + sInterpunkce.Trim();
	  return sText + sInterpunkce.Trim() + ' ';
	}
	public static int PocetVyskytu(string strText, char chZnak)
	{
	  int iPocet = 0;
	  for (int i = 0; i < strText.Length; i++)
	  {
		if (strText[i] == chZnak)
		  iPocet++;
	  }
	  return iPocet;
	}

	/// <summary>
	/// Zjištuje počet výskytů zadaných znaků v textu
	/// </summary>
	/// <param name="strText"></param>
	/// <param name="chZnaky"></param>
	/// <returns></returns>
	public static int PocetVyskytu(string strText, char[] chZnaky)
	{
	  int iPocet = 0;
	  for (int i = 0; i < strText.Length; i++)
	  {
		if (Array.IndexOf(chZnaky, strText[i]) > 0)
		  iPocet++;
	  }
	  return iPocet;
	}

	/// <summary>
	/// Nahradí obyčejné mzery pevnýma za jednopísmennými spojkami a předložkami
	/// </summary>
	/// <param name="sText">Text, u něhož se mají mezery nahradit</param>
	/// <returns>Vrací původní text, který obsahuje pevné mezery podle českého typografického úzu.</returns>
	public static string NahraditPevneMezeryUJendopismennychZnaku(string sText)
	{
	  const string sPismena = "AaIiOoKkSsUuVvZz";
	  const char sPevna = '\u00A0';
	  //const char sPevna = '_';

	  StringBuilder sb = new StringBuilder(sText.Length);
	  int iMezera = -1;
	  int iJednopismenna = -1;

	  for (int i = 0; i < sText.Length; i++)
	  {
		//označím si polohu mezery, přejdu k dalšímu znaku
		if (sText[i] == ' ' || sText[i] == sPevna)
		{
		  //zkontroluju, jestli předchozí dva znaky byly mezera a jednopísmenná předložka/spojka
		  if (iMezera == i - 2 && iJednopismenna == i - 1)
			sb.Append(sPevna);
		  else
			sb.Append(sText[i]);
		  iMezera = i;
		  continue;
		}
		if (iMezera == i - 1 && sPismena.IndexOf(sText[i]) > -1)
		  iJednopismenna = i;
		else
		  iJednopismenna = -1;
		sb.Append(sText[i]);
	  }
	  return sb.ToString();
	}

  }
}
