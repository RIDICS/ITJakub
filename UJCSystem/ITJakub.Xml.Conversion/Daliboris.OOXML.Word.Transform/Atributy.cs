using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Daliboris.OOXML.Word.Transform {
	public class Atributy : ICollection<Atribut> {
		private Dictionary<string, Atribut> mgdcAtributy = new Dictionary<string, Atribut>();

		public bool ObsahujeNazev(string strNazev) {
			return mgdcAtributy.ContainsKey(strNazev);
		}
		public Atribut this[string strNazev] {
			get {
				if (!mgdcAtributy.ContainsKey(strNazev))
					throw new NullReferenceException("Prvek s uvedeným '" + strNazev + "' názvem neexistuje.");
				else
					return mgdcAtributy[strNazev];
			}
			set {
				if (!mgdcAtributy.ContainsKey(strNazev))
					throw new NullReferenceException("Prvek s uvedeným '" + strNazev + "' názvem neexistuje.");
				else
					mgdcAtributy[strNazev] = value;
			}
		}
		#region ICollection<Atribut> Members

		public void Add(Atribut item) {
			mgdcAtributy.Add(item.Nazev, item);
		}

		public void Clear() {
			mgdcAtributy.Clear();
		}

		public bool Contains(Atribut item) {
			return mgdcAtributy.ContainsValue(item);
		}

		public void CopyTo(Atribut[] array, int arrayIndex) {
			mgdcAtributy.Values.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return mgdcAtributy.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool Remove(Atribut item) {
			return mgdcAtributy.Remove(item.Nazev);
		}

		#endregion

		#region IEnumerable<Atribut> Members

		public IEnumerator<Atribut> GetEnumerator() {
			return mgdcAtributy.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		#endregion
	}
}
