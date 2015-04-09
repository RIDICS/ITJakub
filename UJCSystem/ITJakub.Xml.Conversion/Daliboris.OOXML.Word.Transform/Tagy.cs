using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Daliboris.OOXML.Word.Transform {
	public class Tagy : ICollection<Tag> {

		private Dictionary<string, Tag> mgdcTagy = new Dictionary<string, Tag>();

		public Tagy GetTagyByNazev(string strNazev) {
			Tagy tg = new Tagy();
			foreach (Tag t in this) {
				if (t.Nazev == strNazev)
					tg.Add(t);
			}
			return tg;
		}

		public Tag GetTagByID(string strID) {
			if (!ContainsID(strID))
				return null;
			return mgdcTagy[strID];
		}

		public bool ContainsID(string strID) {
			return mgdcTagy.ContainsKey(strID);
		}

		#region ICollection<Tag> Members

		public void Add(Tag item) {
			mgdcTagy.Add(item.Identifikator, item);
		}

		public void Clear() {
			mgdcTagy.Clear();
		}

		public bool Contains(Tag item) {
			return mgdcTagy.ContainsValue(item);
		}

		public void CopyTo(Tag[] array, int arrayIndex) {
			Tag[] tg = mgdcTagy.Select(stl => stl.Value).ToArray();
			tg.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return mgdcTagy.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool Remove(Tag item) {

			return mgdcTagy.Remove(item.Identifikator);
		}

		#endregion

		#region IEnumerable<Tag> Members

		public IEnumerator<Tag> GetEnumerator() {
			return mgdcTagy.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		#endregion
	}
}
