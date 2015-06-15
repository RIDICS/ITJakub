using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daliboris.Texty.Evidence.Rozhrani
{
 public	interface IVydani
	{
	 /// <summary>
	 /// Komerční titul užitý při vydání.
	 /// </summary>
	 string Titul { get; set; }

	 /// <summary>
	 /// Seznam mezninárodních standardních čísel knihy přiřazených pro konkrétní vydání
	 /// </summary>
	 IList<IIsbn> EvidencniCisla { get; set; }

	 /// <summary>
	 /// Způsob využití (pro který bylo dané vydání publikováno)
	 /// </summary>
	 ZpusobVyuziti ZpusobVyuziti { get; set; }


   /// <summary>
   /// Vročení, kdy k vydání došlo
   /// </summary>
	 string Vroceni { get; set; }

	 /// <summary>
	 /// Počet stran A4, kolik bude publikace mít (pro přidělení ISBN)
	 /// </summary>
	 int PocetStran { get; set; }

     /// <summary>
     /// Rozsah stran, který byl ve výstupu publikován (zejména u úryvků v rámci audioknihy)
     /// </summary>
     string Rozsah { get; set; }

     /// <summary>
     /// Interpreti, kteří se podíleli na vytvoření audioknihy.
     /// </summary>
     List<string> Interpreti { get; set; }

     /// <summary>
     /// Seznam audiosoubrů obsahujících nahrávku.
     /// </summary>
     IList<IAudiosoubor> Audiosoubory { get; set; }

     /// <summary>
     /// Pořadí textu v rámci audioknihy
     /// </summary>
     int Poradi { get; set; }
	}
}
