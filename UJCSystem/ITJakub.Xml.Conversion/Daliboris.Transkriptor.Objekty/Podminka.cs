using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Daliboris.Transkripce.Objekty {
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	/// <summary>
	/// Třída popisující podmínky, za nichž platí určité pravidlo.
	/// </summary>
	public class Podminka : IEquatable<Podminka> {
		private string mstrPopis = null;

		/// <summary>
		/// Slovní popis podmínky, za níž platí určité pravidlo.
		/// </summary>
		public string Popis {
			get { return mstrPopis; }
			set { mstrPopis = value; }
		}


		#region IEquatable<Podminka> Members

		/// <summary>
		/// Porovnává rovnost dvou podmínek.
		/// </summary>
		/// <param name="other">Objekt, s nímž se porovnává shoda podmínek.</param>
		/// <returns>Vrací true pokud jsou dvě podmínky shodné; vrací false, pokud se podmínky liší.</returns>
		public bool Equals(Podminka other) {
			if (other == null)
				return false;
			return (this.Popis == other.Popis);
		}

		#endregion
	}
}
