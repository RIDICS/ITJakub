using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Daliboris.OOXML.Word.Transform {
	public class Citace : ICollection<Citac> {
		private Dictionary<string, Citac> mgdcCitace = new Dictionary<string, Citac>();

		public void AktualizujHodnoty(string sPrvek) {
			foreach (Citac ct in this) {
				if (ct.Inkrementator == sPrvek)
					ct.Inkrementovat();
				if (ct.Resetator == sPrvek)
					ct.Resetovat();
			}
		}

		public void ResetujHodnoty() {
			foreach (Citac ct in this) {
				ct.Resetovat();
			}
		}

		#region ICollection<Citac> Members

		public void Add(Citac item) {
			mgdcCitace.Add(item.Nazev, item);
		}

		public void Clear() {
			mgdcCitace.Clear();
		}

		public bool Contains(Citac item) {
			return mgdcCitace.ContainsValue(item);
		}

		public void CopyTo(Citac[] array, int arrayIndex) {
			Citac[] tg = mgdcCitace.Select(stl => stl.Value).ToArray();
			tg.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return mgdcCitace.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool Remove(Citac item) {
			return mgdcCitace.Remove(item.Nazev);
		}

		#endregion

		#region IEnumerable<Citac> Members

		public IEnumerator<Citac> GetEnumerator() {
			return mgdcCitace.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		#endregion

	}
}
