using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Daliboris.Transkripce.Objekty {
	/// <summary>
	/// Pravidlo, které se aplikuje
	/// </summary>
	[XmlRoot("p")]
	[XmlType("p")]
	[DebuggerDisplay("Pravidlo: {Jazyk ?? \"\"} {Transliterace} == {Transkripce}")]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(IPravidlo))]
	public class Pravidlo : IEquatable<Pravidlo>, IComparable<Pravidlo>, IPravidlo {
		private string mstrTranskripce = null;
		private string mstrTransliterace = null;
		private string mstrJazyk = null;
		private string mstrUrceni = null;
		private Doklady mdklDoklady = new Doklady();


		public Pravidlo() { }
		public Pravidlo(string strTransliterace, string strTranskripce) {
			mstrTransliterace = strTransliterace;
			mstrTranskripce = strTranskripce;
		}


		public Pravidlo(string strTransliterace, string strTranskripce, string strJazyk)
			: this(strTransliterace, strTranskripce) {
			mstrJazyk = strJazyk;
		}
		/// <summary>
		/// Zkratka jazyka, pro který je pravidlo určeno. Kromě zavedených zkratek normy ISO 
		/// můžou existovat i zkratky vlastní (např. ocz pro old chzech ap.)
		/// </summary>
		/// 
		[XmlAttribute("jazyk")]
		public string Jazyk {
			get { return mstrJazyk; }
			set { mstrJazyk = value; }
		}


		/// <summary>
		/// Transliterovaná podoba
		/// </summary>
		/// 
		[XmlAttribute("tl")]
		public string Transliterace {
			get { return mstrTransliterace; }
			set { mstrTransliterace = value; }
		}

		/// <summary>
		/// Transkribovaná podoba odpovídající transliterovanému zápisu
		/// </summary>
		/// 
		[XmlAttribute("tk")]
		public string Transkripce {
			get { return mstrTranskripce; }
			set { mstrTranskripce = value; }
		}

		[XmlArray(ElementName = "doklady", IsNullable = true)]
		[XmlArrayItem("d")]
		public Doklady Doklady {
			get { return mdklDoklady; }
			set { mdklDoklady = value; }
		}

		/// <summary>
		/// Při serializaci do Xml nezahrne prázdný element, pokud neobsahuje žádné prvky
		/// </summary>
		[XmlIgnore]
		public bool DokladySpecified {
			get { return mdklDoklady.Count > 0; }
		}


		//public string Priklad {
		//   get { return mstrPriklad; }
		//   set { mstrPriklad = value; }
		//}

		/// <summary>
		/// Pro jaký text (památku, časové období ap.) je providlo určeno
		/// </summary>
		/// 
		[XmlElement("urceni")]
		public string Urceni {
			get { return mstrUrceni; }
			set { mstrUrceni = value; }
		}

        /// <summary>
        /// Zda je pravidlo definováno jako regulární výraz
        /// </summary>
        [XmlAttribute("isRegularExpression")]
	    public bool IsRegularExpression { get; set; }

	    #region IEquatable<Pravidlo> Members

		/// <summary>
		/// Porovnává rovnost dvou pravidel.
		/// </summary>
		/// <param name="other">Objekt, s nímž se porovnává shoda pravidel.</param>
		/// <returns>Vrací true pokud jsou dvě pravidla shodná; vrací false, pokud se pravidla liší.</returns>
		public bool Equals(Pravidlo other) {
			if (other == null)
				return false;
			bool bJeRovno = (this.Transliterace == other.Transliterace) &&
				(this.Transkripce == other.Transkripce) &&
				 (this.Jazyk == other.Jazyk) &&
				(this.Urceni == other.Urceni);
			return bJeRovno;
		}

		#endregion

		#region IComparable<Pravidlo> Members

		public int CompareTo(Pravidlo other) {
			string sText = other.Transliterace ?? "" + other.Transkripce ?? "" + other.Jazyk ?? "" + other.Urceni ?? "";
			return (mstrTransliterace ?? "" + mstrTranskripce ?? "" + mstrJazyk ?? "" + mstrUrceni ?? "").CompareTo(sText);
		}

		#endregion

		public override bool Equals(object obj) {
			Pravidlo prv = obj as Pravidlo;
			if (prv == null)
				return false;
			return this.Equals(prv);
		}

		public static bool operator ==(Pravidlo pravidlo1, Pravidlo pravidlo2) {
			if (object.ReferenceEquals(pravidlo1, pravidlo2))
				return true;
			if (object.ReferenceEquals(pravidlo1, null))
				return false;
			if (object.ReferenceEquals(pravidlo2, null))
				return false;
			return pravidlo1.Equals(pravidlo2);
		}
		public static bool operator !=(Pravidlo pravidlo1, Pravidlo pravidlo2) {
			if (object.ReferenceEquals(pravidlo1, pravidlo2))
				return false;
			if (object.ReferenceEquals(pravidlo1, null))
				return true;
			if (object.ReferenceEquals(pravidlo2, null))
				return true;
			return !pravidlo1.Equals(pravidlo2);
		}
		public override int GetHashCode() {
			string sText = mstrTransliterace + mstrTranskripce + mstrJazyk + mstrUrceni;
			return sText.GetHashCode();
		}

		#region IPravidlo - členové

		public int CompareTo(IPravidlo other) {
			return CompareTo(other as Pravidlo);
		}

		IDoklady IPravidlo.Doklady {
			get {
				return (IDoklady)Doklady;
			}
			set {
				mdklDoklady = value as Doklady;
			}
		}

		public bool Equals(IPravidlo other) {
			return Equals(other as Pravidlo);
		}

		#endregion
	}
}
