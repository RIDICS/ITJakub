using System;
using System.Collections.Generic;
using System.Text;

namespace Daliboris.Pomucky.Databaze.Zaznamy {

	/// <summary>
	/// Rozhraní objektů, které ukládají svá data do jednoho textového řádku
	/// </summary>
	/// <typeparam name="T">Třída, jejíž informace se uloží jako jeden textový záznam.</typeparam>
	public interface IZaznam<T> : IComparable<T> {
		void NactiZaznam(string strZaznam);
		void NactiZaznam(string strZaznam, char chOddelovacPoli);
		void NactiZaznam(string strZaznam, char chOddelovacPoli, char chOddelovacHodnot);
		string VytvorZaznam();
		string VytvorZaznam(char chOddelovacPoli);
		string VytvorZaznam(char chOddelovacPoli, char chOddelovacHodnot);
		char OddelovacPoli { get; set; }
		char OddelovacHodnot { get; set; }
	}
}
