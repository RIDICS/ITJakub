using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Daliboris.Transkripce.Objekty
{
	[DebuggerDisplay("Korelat: {Transliterace.Text} == {Transkripce.Text}")]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public class Korelat : IEquatable<Korelat>, IComparable<Korelat> {
		private Segment msgTranskripce = new Segment();
		private Segment msgTransliterace = new Segment();

		public Korelat() { }
		public Korelat(Segment sgTransliterace, Segment sgTranskripce) {
			msgTransliterace = sgTransliterace;
			msgTranskripce = sgTranskripce;
		}

		public Segment Transkripce
		{
			get { return msgTranskripce; }
			set { msgTranskripce = value; }
		}

		public Segment Transliterace
		{
			get { return msgTransliterace; }
			set { msgTransliterace = value; }
		}

		#region IEquatable<Korelat> Members

		public bool Equals(Korelat other)
		{
			if (other == null) return false;
			return (this.Transkripce.Text == other.Transkripce.Text) &&
			       (this.Transliterace.Text == other.Transliterace.Text);
		}
		
		#endregion

		public static bool operator == (Korelat korelat1, Korelat korelat2) {
			if (object.ReferenceEquals(korelat1, korelat2))
				return true;
			if (object.ReferenceEquals(korelat1, null))
				return false;
			if (object.ReferenceEquals(korelat2, null))
				return false;
			return korelat1.Equals(korelat2);
		}


		public static bool operator !=(Korelat korelat1, Korelat korelat2) {
			return (!(korelat1 == korelat2));
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override bool Equals(object obj) {
			Korelat kr = obj as Korelat;
			if (obj == null)
				return false;

			if (!msgTranskripce.Equals(kr.Transkripce))
				return false;
			if (!msgTransliterace.Equals(kr.Transliterace))
				return false;
			return true;
		}

		

		#region IComparable<Korelat> - členové

		public int CompareTo(Korelat other) {
			int i = 0;
			if (other == null)
				return 1;
			i	= msgTransliterace.CompareTo(other.Transliterace);
			if (i == 0) {
				i = msgTranskripce.CompareTo(other.Transkripce);
			}
			return i;
		}

		#endregion
	}
}