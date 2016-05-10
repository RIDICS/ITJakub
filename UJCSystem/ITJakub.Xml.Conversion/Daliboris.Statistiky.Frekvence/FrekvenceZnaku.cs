using System;
using System.Collections.Generic;
using System.Text;

namespace Daliboris.Statistiky.Frekvence {
	public class FrekvenceZnaku : IEquatable<FrekvenceZnaku>, IComparable<FrekvenceZnaku>, IComparable {
		private char[] machPocitaneZnaky = null;
		private string mstrText = null;
		private bool mblnZaleziNaPoradi = false;
		//vzít v potaz ještě hranice slov, popř. hranice textu

		public bool ZaleziNaPoradi {
			get { return mblnZaleziNaPoradi; }
			set { mblnZaleziNaPoradi = value; }
		}
		private string mstrPoradiZnaku = "";

		public string PoradiZnaku {
			get { return mstrPoradiZnaku; }
			set { mstrPoradiZnaku = value; }
		}
		private Dictionary<char, int> mgdcPoctyZnaku = null;

		public FrekvenceZnaku() { }
		public FrekvenceZnaku(char[] achPocitaneZnaky) {
			PrevestZnakyDoSlovniku(achPocitaneZnaky);
		}
		public FrekvenceZnaku(char[] achPocitaneZnaky, string strText)
			: this(achPocitaneZnaky) {
			mstrText = strText;
		}

		public FrekvenceZnaku(char[] achPocitaneZnaky, string strText, bool blnZaleziNaPoradi)
			: this(achPocitaneZnaky, strText) {
			mblnZaleziNaPoradi = blnZaleziNaPoradi;
		}

		public void SpocitejFrekvenci() {
			if (mstrText == null || machPocitaneZnaky == null)
				throw new ArgumentNullException("Nejsou nastaveny všechny potřebné argumenty.");

			if (mstrText.IndexOfAny(machPocitaneZnaky) > -1) {
				StringBuilder sbPoradi = new StringBuilder(mstrText.Length);
				int i = 0;
				foreach (char c in mstrText) {
					if (mgdcPoctyZnaku.ContainsKey(c)) {
						mgdcPoctyZnaku[c]++;
						if (mblnZaleziNaPoradi)
							sbPoradi.Append(c);
					}
				}
				if (mblnZaleziNaPoradi)
					mstrPoradiZnaku = sbPoradi.ToString();
			}
		}

		private void SpocitejFrekvenci(string strText) {
			mstrText = strText;
			SpocitejFrekvenci();
		}

		private void SpocitejFrekvenci(char[] achPocitaneZnaky, string strText) {
			PrevestZnakyDoSlovniku(achPocitaneZnaky);
			SpocitejFrekvenci(strText);
		}
		private void SpocitejFrekvenci(char[] achPocitaneZnaky, string strText, bool blnZaleziNaPoradi) {
			mblnZaleziNaPoradi = blnZaleziNaPoradi;
			SpocitejFrekvenci(achPocitaneZnaky, strText);
		}

		private void PrevestZnakyDoSlovniku(char[] achPocitaneZnaky) {
			machPocitaneZnaky = achPocitaneZnaky;
			PrevestZnakyDoSlovniku();
		}
		private void PrevestZnakyDoSlovniku() {
			mgdcPoctyZnaku = new Dictionary<char, int>(machPocitaneZnaky.Length);

			foreach (char c in machPocitaneZnaky) {
				if (!mgdcPoctyZnaku.ContainsKey(c))
					mgdcPoctyZnaku.Add(c, 0);
			}
		}

		public string Text {
			get {
				return mstrText;
			}
		}
		public Dictionary<char, int> Frekvence {
			get { return mgdcPoctyZnaku; }
		}


		#region IEquatable<FrekvenceZnaku> Members

		public bool Equals(FrekvenceZnaku other) {
			if (other == null)
				return false;
			Dictionary<char, int> gdc = other.Frekvence;
			if (gdc.Count != mgdcPoctyZnaku.Count)
				return false;
			if (mblnZaleziNaPoradi && other.ZaleziNaPoradi)
				return mstrPoradiZnaku.Equals(other.PoradiZnaku);
			foreach (KeyValuePair<char, int> kvp in mgdcPoctyZnaku) {
				if (gdc.ContainsKey(kvp.Key)) {
					if (kvp.Value != gdc[kvp.Key])
						return false;
				}
				else { return false; }
			}
			return true;
		}
		#endregion

		public override string ToString() {
			StringBuilder sb = new StringBuilder(mgdcPoctyZnaku.Count * 4);
			foreach (KeyValuePair<char, int> kvp in mgdcPoctyZnaku) {
				sb.AppendFormat("{0} = {1}\t", kvp.Key, kvp.Value);
			}
			if (mblnZaleziNaPoradi)
				sb.Append("==> " + mstrPoradiZnaku);
			return sb.ToString();
		}

		public override bool Equals(object obj) {
			if (obj is FrekvenceZnaku) {
				return (this.Equals(obj as FrekvenceZnaku));
			}
			else
				return false;
		}

		public override int GetHashCode() {
			if (mblnZaleziNaPoradi)
				return mstrPoradiZnaku.GetHashCode();
			int iHashCode = 0;
			foreach (KeyValuePair<char, int> kvp in mgdcPoctyZnaku) {
				iHashCode += kvp.Key.GetHashCode() + kvp.Value.GetHashCode();
			}
			return iHashCode;
		}


		#region IComparable<FrekvenceZnaku> Members

		public int CompareTo(FrekvenceZnaku other) {
			Dictionary<char, int> gdc = other.Frekvence;
			if (gdc.Count != mgdcPoctyZnaku.Count)
				return 1;
			if (mblnZaleziNaPoradi)
				return mstrPoradiZnaku.CompareTo(other.PoradiZnaku);
			foreach (KeyValuePair<char, int> kvp in mgdcPoctyZnaku) {
				if (gdc.ContainsKey(kvp.Key)) {
					if (kvp.Value != gdc[kvp.Key])
						return kvp.Value.CompareTo(gdc[kvp.Key]);
				}
				else { return 1; }
			}
			return 0;
		}

		#endregion

		#region IComparable Members

		public int CompareTo(object obj) {
			if (obj is FrekvenceZnaku) {
				FrekvenceZnaku fz = obj as FrekvenceZnaku;
				return this.CompareTo(fz);
			}
			else {
				throw new ArgumentException("Porovnávaný objekt není typu FrekvenceZnaku");
			}
			throw new NotImplementedException();
		}

		#endregion
	}
}
