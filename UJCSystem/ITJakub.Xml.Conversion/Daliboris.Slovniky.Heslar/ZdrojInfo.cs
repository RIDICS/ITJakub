using System.Diagnostics;

namespace Daliboris.Slovniky {
  [DebuggerDisplay("{Identifikator}, {Zkratka}, {Akronym}, {Pomocny}")]
  public class ZdrojInfo {
   /// <summary>
   /// Identifikáotr zdroje, jeho ID v databázi
   /// </summary>
   /// <example>1</example>
 	public int Identifikator { get; set; }

   /// <summary>
   /// Zkratka zdroje, s diakritikou.
   /// </summary>
   /// <example>HesStčS</example>
 	public string Zkratka { get; set; }

   /// <summary>
   /// Zkratka zdroje bez diaktiritiky
   /// </summary>
   /// <example>HesStcS</example>
 	public string Akronym { get; set; }

   /// <summary>
   /// Zda se jedná o pomocný zdroj, obvykle o variantu (pojmenování) základního zdroje.
   /// </summary>
 	public bool Pomocny { get; set; }
  }
}
