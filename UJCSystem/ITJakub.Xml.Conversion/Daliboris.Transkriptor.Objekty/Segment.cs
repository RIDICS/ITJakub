using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Daliboris.Transkripce.Objekty
{
	[DebuggerDisplay("{mstrText} ({mintPozice}, {mintPocetZnaku})")]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public class Segment : IEquatable<Segment>, IComparable<Segment> {
		private int mintPozice;
		private int mintPocetZnaku;
		private string mstrText;

		public Segment() { }
		public Segment(string strText): this(0, strText.Length, strText) { }
		public Segment(int intPozice, int intPocetZnaku, string strText) {
			mintPozice = intPozice;
			mintPocetZnaku = intPocetZnaku;
			mstrText = strText;
		}

		public int Pozice
		{
			get { return mintPozice; }
			set { mintPozice = value; }
		}

		public int PocetZnaku
		{
			get { return mintPocetZnaku; }
			set { mintPocetZnaku = value; }
		}

		public string Text
		{
			get { return mstrText; }
			set { mstrText = value; }
		}




		#region IEquatable<Segment> - členové

		public bool Equals(Segment other) {
			if (other == null)
				return false;
			if (mintPocetZnaku != other.PocetZnaku)
				return false;
			if (mintPozice != other.Pozice)
				return false;
			if (mstrText == null && other.Text == null)
				return true;
			return mstrText.Equals(other.Text);
		}

		#endregion

		public static bool operator == (Segment segment1, Segment segment2) {
			if (object.ReferenceEquals(segment1, segment2))
				return true;
			if (object.ReferenceEquals(segment1, null))
				return false;
			if (object.ReferenceEquals(segment2, null))
				return false;
			return segment1.Equals(segment2);
		}

		public static bool operator != (Segment segment1, Segment segment2) {
			return (!(segment1 == segment2));
		}


		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override bool Equals(object obj) {
			Segment sg = obj as Segment;
			if (sg == null)
				return false;
			if (mintPocetZnaku != sg.PocetZnaku)
				return false;
			if (mintPozice != sg.Pozice)
				return false;
			if (mstrText != sg.Text)
				return false;
			return true;
		}

		#region IComparable<Segment> - členové

		public int CompareTo(Segment other) {
			int i = 0;
			if (other == null)
				return 1;
			i	= String.Compare(mstrText, other.Text);
			if (i == 0) {
				i = mintPocetZnaku.CompareTo(other.PocetZnaku);
				if (i== 0) {
					i = mintPozice.CompareTo(other.Pozice);
				}
			}
			return i;
		}

		#endregion
	}
}