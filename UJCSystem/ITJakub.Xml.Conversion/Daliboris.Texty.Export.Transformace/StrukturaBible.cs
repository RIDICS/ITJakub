using System;
using System.Diagnostics;

namespace Daliboris.Texty.Export {

	public enum VypisStruktury
	{
		Kniha,
		Kapitola,
		Vers
	}

  [DebuggerDisplay("{Kniha,nq} {Kapitola,nq},{Vers,nq}")]
  public class StrukturaBible {
	public string Kniha { get; set; }
	public string Kapitola { get; set; }
	public string Vers { get; set; }
	  /// <summary>
	  /// Identifikátor použitý v XML jako začátek elementu xml:id (který nemůže začínat číslicí).
	  /// </summary>
	  public string Identifikator { get; set; }

	  public const string VychoziIdentifikator = "Bibl.";

	public StrukturaBible()
	{
		Identifikator = VychoziIdentifikator;
	}

	public StrukturaBible(string strKniha) : this() {
	  Kniha = strKniha;
	}

	public StrukturaBible(string strKniha, string strKapitola)
	  : this(strKniha) {
	  Kapitola = strKapitola;
	}

	public StrukturaBible(string strKniha, string strKapitola, string strVers)
	  : this(strKniha, strKapitola) {
	  Vers = strVers;
	}
	
	/// <summary>
	/// Získává hodnotu atributu pro biblickou knihu, číslo a vers oddelěním jedinečného identifikátoru
	/// </summary>
	/// <param name="sAtribut">Hodnota atributu</param>
	/// <returns>Vrací zkratku biblické knihy, číslo kapitoly nebo verše</returns>
	public static string ZiskejUdajZAtributu(string sAtribut) {
	  return sAtribut.Substring(sAtribut.LastIndexOf('.') + 1);
	}

	  private static string IdentifikatorXml(string strKniha) {
		return strKniha.Replace(" ", "");
	}

	  private static string IdentifikatorXml(string strKniha, string strKapitola) {
	  return String.Format("{0}.{1}", IdentifikatorXml(strKniha), strKapitola);
	}

	  private static string IdentifikatorXml(string strKniha, string strKapitola, string strVers) {
	  return String.Format("{0}.{1}", IdentifikatorXml(strKniha, strKapitola), strVers);
	}

	public string OznaceniBiblickehoMista() {
	  if (Vers != null)
		return String.Format("{0} {1},{2}", Kniha, Kapitola, Vers);
	  if (Kapitola != null)
		return String.Format("{0} {1}", Kniha, Kapitola);
	  return String.Format("{0}", Kniha);
	}

	/// <summary>
	/// Identifikátor biblického místa ve formátu pro atribut XML
	/// </summary>
	/// <returns>Vrací jednotlivé údaje oddělené tečkou</returns>
	public string IdentifikatorXml() {
		return Identifikator + IdentifikatorXml(Kniha, Kapitola, Vers);
	}

	  public string IdentifikatorXml(VypisStruktury vypis)
	  {
		  switch (vypis)
		  {
			  case VypisStruktury.Kniha:
				  return Identifikator + IdentifikatorXml(Kniha);
				  break;
			  case VypisStruktury.Kapitola:
				  return Identifikator + IdentifikatorXml(Kniha, Kapitola);
				  break;
			  case VypisStruktury.Vers:
				  return Identifikator + IdentifikatorXml(Kniha, Kapitola, Vers);
				  break;
			  default:
				  throw new ArgumentOutOfRangeException("vypis");
		  }
	  }

  }
}
