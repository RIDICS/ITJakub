using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daliboris.Statistiky.Frekvence {
	public class FrekvencePrvku<T, K> : IEquatable<FrekvencePrvku<T, K>>, IComparable<FrekvencePrvku<T, K>>, IComparable 
		where K: IEnumerable<T>
		{
		private List<T> mglstPocitanePrvky = new List<T>();
		private bool mblnZaleziNaPoradi = false;
		private IEnumerable<T> mgenZdrojPrvku = null;
		private string mstrText = null;


		public bool ZaleziNaPoradi {
			get { return mblnZaleziNaPoradi; }
			set { mblnZaleziNaPoradi = value; }
		}
		private string mstrPoradiPrvku = "";

		public string PoradiPrvku {
			get { return mstrPoradiPrvku; }
			set { mstrPoradiPrvku = value; }
		}
		private Dictionary<T, int> mgdcPoctyPrvku = null;

				public FrekvencePrvku() { }
		public FrekvencePrvku(T[] achPocitanePrvky) {
			PrevestPrvkyDoSlovniku(achPocitanePrvky);
		}
		public FrekvencePrvku(T[] achPocitanePrvky, string strText)
			: this(achPocitanePrvky) {
			mstrText = strText;
		}

				public FrekvencePrvku(T[] achPocitanePrvky, string strText, bool blnZaleziNaPoradi)
			: this(achPocitanePrvky, strText) {
			mblnZaleziNaPoradi = blnZaleziNaPoradi;
		}

			private void PrevestPrvkyDoSlovniku(T[] achPocitanePrvky) {
			mglstPocitanePrvky = new List<T>(achPocitanePrvky);
			PrevestPrvkyDoSlovniku();
		}
		private void PrevestPrvkyDoSlovniku() {
			mgdcPoctyPrvku = new Dictionary<T, int>(mglstPocitanePrvky.Count);

			foreach (T c in mglstPocitanePrvky) {
				if (!mgdcPoctyPrvku.ContainsKey(c))
					mgdcPoctyPrvku.Add(c, 0);
			}
		}

		public string Text {
			get {
				return mstrText;
			}
		}
		public Dictionary<T, int> Frekvence {
			get { return mgdcPoctyPrvku; }
		}


		public void SpocitejFrekvenci() {
			/*
			if (mstrText == null || mglstPocitanePrvky == null)
				throw new ArgumentNullException("Nejsou nastaveny všechny potřebné argumenty.");

			if (typeof(T) is char) {
				char[] machPocitaneZnaky = mglstPocitanePrvky.ToArray<char>();
				if (mstrText.IndexOfAny(machPocitaneZnaky) > -1) {
					StringBuilder sbPoradi = new StringBuilder(mstrText.Length);
					int i = 0;
					foreach (char c in mstrText) {
						if (mgdcPoctyPrvku.ContainsKey(c)) {
							mgdcPoctyPrvku[c]++;
							if (mblnZaleziNaPoradi)
								sbPoradi.Append(c);
						}
					}
					if (mblnZaleziNaPoradi)
						mstrPoradiZnaku = sbPoradi.ToString();
				}
			}
			else if (typeof(T) is string) {
				foreach (string item in mglstPocitanePrvky) {
					StringBuilder sbPoradi = new StringBuilder(mstrText.Length);
					string[] aSlova = Daliboris.Text.Slova.RozdelitTextNaSlova(mstrText);
					foreach (string sSlovo in aSlova) {
						if (mgdcPoctyPrvku.ContainsKey(sSlovo)) {
							mgdcPoctyPrvku[sSlovo]++;
							if (mblnZaleziNaPoradi)
								sbPoradi.Append(sSlovo);
						}
					}
				}
			}
			*/
		}
		/*
		#region IEquatable<FrekvencePrvku<T>> Members

		public bool Equals(FrekvencePrvku<T> other) {
			if (other == null)
				return false;
			Dictionary<T, int> gdc = other.Frekvence;
			if (gdc.Count != mgdcPoctyPrvku.Count)
				return false;
			if (mblnZaleziNaPoradi && other.ZaleziNaPoradi)
				return mstrPoradiPrvku.Equals(other.PoradiPrvku);
			foreach (KeyValuePair<T, int> kvp in mgdcPoctyPrvku) {
				if (gdc.ContainsKey(kvp.Key)) {
					if (kvp.Value != gdc[kvp.Key])
						return false;
				}
				else { return false; }
			}
			return true;
		}
		#endregion
		*/
		public override string ToString() {
			StringBuilder sb = new StringBuilder(mgdcPoctyPrvku.Count * 4);
			foreach (KeyValuePair<T, int> kvp in mgdcPoctyPrvku) {
				sb.AppendFormat("{0} = {1}\t", kvp.Key, kvp.Value);
			}
			if (mblnZaleziNaPoradi)
				sb.Append("==> " + mstrPoradiPrvku);
			return sb.ToString();
		}
		/*
		public override bool Equals(object obj) {
			if (obj is FrekvencePrvku<T>) {
				return (this.Equals(obj as FrekvencePrvku<T>));
			}
			else
				return false;
		}
		*/
		public override int GetHashCode() {
			if (mblnZaleziNaPoradi)
				return mstrPoradiPrvku.GetHashCode();
			int iHashCode = 0;
			foreach (KeyValuePair<T, int> kvp in mgdcPoctyPrvku) {
				iHashCode += kvp.Key.GetHashCode() + kvp.Value.GetHashCode();
			}
			return iHashCode;
		}

		/*
		#region IComparable<FrekvencePrvku> Members

		public int CompareTo(FrekvencePrvku<T> other) {
			Dictionary<T, int> gdc = other.Frekvence;
			if (gdc.Count != mgdcPoctyPrvku.Count)
				return 1;
			if (mblnZaleziNaPoradi)
				return mstrPoradiPrvku.CompareTo(other.PoradiPrvku);
			foreach (KeyValuePair<T, int> kvp in mgdcPoctyPrvku) {
				if (gdc.ContainsKey(kvp.Key)) {
					if (kvp.Value != gdc[kvp.Key])
						return kvp.Value.CompareTo(gdc[kvp.Key]);
				}
				else { return 1; }
			}
			return 0;
		}

		#endregion

		*/ 

		/*
		#region IComparable Members

		public int CompareTo(object obj) {
			if (obj is FrekvencePrvku<T>) {
				FrekvencePrvku<T> fz = obj as FrekvencePrvku<T>;
				return this.CompareTo(fz);
			}
			else {
				throw new ArgumentException("Porovnávaný objekt není typu FrekvencePrvku");
			}
			throw new NotImplementedException();
		}

		#endregion
		*/
		/*
		#region IEquatable<FrekvencePrvku<T>> Members

		bool IEquatable<FrekvencePrvku<T>>.Equals(FrekvencePrvku<T> other) {
			if (other == null)
				return false;
			Dictionary<T, int> gdc = other.Frekvence;
			if (gdc.Count != mglstPocitanePrvky.Count)
				return false;
			if (mblnZaleziNaPoradi && other.ZaleziNaPoradi)
				return mstrPoradiPrvku.Equals(other.PoradiPrvku);
			foreach (KeyValuePair<T, int> kvp in mgdcPoctyPrvku) {
				if (gdc.ContainsKey(kvp.Key)) {
					if (kvp.Value != gdc[kvp.Key])
						return false;
				}
				else { return false; }
			}
			return true;
		}
		
		#endregion
		*/ 

		/*
		#region IComparable<FrekvencePrvku<T>> Members

		int IComparable<FrekvencePrvku<T>>.CompareTo(FrekvencePrvku<T> other) {
			Dictionary<T, int> gdc = other.Frekvence;
			if (gdc.Count != mgdcPoctyPrvku.Count)
				return 1;
			if (mblnZaleziNaPoradi)
				return mstrPoradiPrvku.CompareTo(other.PoradiPrvku);
			foreach (KeyValuePair<T, int> kvp in mgdcPoctyPrvku) {
				if (gdc.ContainsKey(kvp.Key)) {
					if (kvp.Value != gdc[kvp.Key])
						return kvp.Value.CompareTo(gdc[kvp.Key]);
				}
				else { return 1; }
			}
			return 0;

		}

		#endregion
		 */

		#region IComparable<FrekvencePrvku<T,K>> Members

		public int CompareTo(FrekvencePrvku<T, K> other) {
			throw new NotImplementedException();
		}

		#endregion

		#region IEquatable<FrekvencePrvku<T,K>> Members

		public bool Equals(FrekvencePrvku<T, K> other) {
			throw new NotImplementedException();
		}

		#endregion

		#region IComparable Members

		public int CompareTo(object obj) {
			throw new NotImplementedException();
		}

		#endregion
	}
}
