using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Daliboris.Transkripce.Objekty {

	/// <summary>
	/// Kolekce více podmínek.
	/// </summary>
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public class Podminky : ICollection<Podminka> {


		private bool mblnIsReadOnly = false;
		private List<Podminka> mglsPodminky = new List<Podminka>();

		#region ICollection<Podminka> Members

		public void Add(Podminka item) {
			mglsPodminky.Add(item);
		}

		public void Clear() {
			mglsPodminky.Clear();
		}

		public bool Contains(Podminka item) {
			return mglsPodminky.Contains(item);
		}

		public void CopyTo(Podminka[] array, int arrayIndex) {
			mglsPodminky.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return mglsPodminky.Count; }
		}

		public bool IsReadOnly {
			get { return mblnIsReadOnly; }
		}

		public bool Remove(Podminka item) {
			return mglsPodminky.Remove(item);
		}

		#endregion

		#region IEnumerable<Podminka> Members

		public IEnumerator<Podminka> GetEnumerator() {
			return mglsPodminky.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		#endregion

	}
}
