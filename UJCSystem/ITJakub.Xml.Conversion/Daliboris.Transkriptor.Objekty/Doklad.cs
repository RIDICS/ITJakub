using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Daliboris.Transkripce.Objekty
{
	/// <summary>
	/// Doklad na určitý jev. Sestává z transliterace a odpovídající transkripce.
	/// </summary>
	[XmlRoot("doklad")]
	[XmlType("d")]
	[DebuggerDisplay("{mstrTransliterace} == {mstrTranskripce} {mintPocet}×")]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(IDoklad))]
	public class Doklad :IEquatable<Doklad>, IComparable<Doklad>, IDoklad {
		private string mstrTranskripce = null;
		private string mstrTransliterace = null;
		private int mintPocet = 1;


		public Doklad() { }
		public Doklad(string strTransliterace, string strTranskripce) {
			mstrTransliterace = strTransliterace;
			mstrTranskripce = strTranskripce;
		}

		public Doklad(string strTransliterace, string strTranskripce, int intPocet) : this(strTransliterace, strTranskripce)  {
			mintPocet = intPocet;
		}
		/// <summary>
		/// Transliterovaný text.
		/// </summary>
		[XmlAttribute("tl")]
		public string Transliterace {
			get { return mstrTransliterace; }
			set { mstrTransliterace = value; }
		}

		/// <summary>
		/// Transkribovaný text.
		/// </summary>
		[XmlAttribute("tk")]
		public string Transkripce {
			get { return mstrTranskripce; }
			set { mstrTranskripce = value; }
		}

		/// <summary>
		/// Počet dokladů.
		/// </summary>
		[XmlAttribute("i")]
		public int Pocet {
			get { return mintPocet; }
			set { mintPocet = value; }
		}


		//public override bool Equals(object obj) {
		//   return this.JeRovno(obj);
		//}

		/// <summary>
		/// Slouží k určení, jestli jsou dva doklady ekvivalentí.
		/// </summary>
		/// <param name="other">Druhý doklad, s nímž se porovnává.</param>
		/// <returns>Doklady jsou si ekvivalentní, pokud jsou transliterace a transkripce obou dokladů shodné.</returns>
		public bool Equals(Doklad other)
		{
			if (other == null)
				return false;
			bool bJeRovno = (this.Transliterace == other.Transliterace) &&
			                (this.Transkripce == other.Transkripce);
			return bJeRovno;
		}

		/// <summary>
		/// Slouží k porovnání dvou dokladů. Srovnává se textový řetězec tvořený transliterací + transkripcí.
		/// </summary>
		/// <param name="other">Druhý doklad, s nímž se porovnává.</param>
		/// <returns></returns>
		public int CompareTo(Doklad other)
		{
			string sText = other.Transliterace ?? "" + other.Transkripce ?? "";
			return (mstrTransliterace ?? "" + mstrTranskripce ?? "").CompareTo(sText);

		}

		#region IDoklad - členové

		public int CompareTo(IDoklad other) {
			return CompareTo(other as Doklad);
		}

		public bool Equals(IDoklad other) {
			return Equals(other as Doklad);
		}

		#endregion
	}
}