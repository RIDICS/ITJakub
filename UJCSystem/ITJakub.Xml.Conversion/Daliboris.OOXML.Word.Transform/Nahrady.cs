using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Daliboris.OOXML.Word.Transform {
	public class Nahrady : ICollection<Nahrada> {
		private List<Nahrada> mglstNahrady = new List<Nahrada>();

		#region ICollection<Nahrada> Members

		public void Add(Nahrada item) {
			mglstNahrady.Add(item);
		}

		public void Clear() {
			mglstNahrady.Clear();
		}

		public bool Contains(Nahrada item) {
			return mglstNahrady.Contains(item);
		}

		public void CopyTo(Nahrada[] array, int arrayIndex) {
			mglstNahrady.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return mglstNahrady.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool Remove(Nahrada item) {
			return mglstNahrady.Remove(item);
		}

		#endregion

		#region IEnumerable<Nahrada> Members

		public IEnumerator<Nahrada> GetEnumerator() {
			return mglstNahrady.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		#endregion
	}
}
