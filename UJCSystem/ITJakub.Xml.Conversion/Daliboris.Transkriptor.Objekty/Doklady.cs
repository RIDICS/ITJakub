using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;

namespace Daliboris.Transkripce.Objekty {
	/// <summary>
	/// Skupina dokladů k určitému jevu.
	/// </summary>
	[XmlRoot("doklady")]
		//[XmlArrayItem("doklad")]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(IDoklady))]
	public class Doklady : ICollection<Doklad>, IDoklady {

		private bool mblnIsReadOnly = false;
		private List<Doklad> mglsDoklady = new List<Doklad>();

		#region ICollection<Doklad> Members

		public void Add(Doklad item) {
			mglsDoklady.Add(item);
		}

		public void Clear() {
			mglsDoklady.Clear();
		}

		public bool Contains(Doklad item) {
			return mglsDoklady.Contains(item);
		}

		public void CopyTo(Doklad[] array, int arrayIndex) {
			mglsDoklady.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return mglsDoklady.Count; }
		}

		public bool IsReadOnly {
			get { return mblnIsReadOnly; }
		}

		public bool Remove(Doklad item) {
			return mglsDoklady.Remove(item);
		}

		#endregion

		public Doklad this[int index] {
			get { return mglsDoklady[index]; }
			set { mglsDoklady[index] = value; }
		}
		public Doklad Get(Doklad item) {
			foreach (Doklad dkl in mglsDoklady) {
				if (item.Equals(dkl))
					return dkl;
			}
			return null;
		}

		public void Sort() {
			mglsDoklady.Sort();
		}



		#region IEnumerable<Doklad> Members

		public IEnumerator<Doklad> GetEnumerator() {
			return mglsDoklady.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		#endregion



		#region IDoklady - členové

		public void Add(IDoklad item) {
			throw new NotImplementedException();
		}

		public bool Contains(IDoklad item) {
			throw new NotImplementedException();
		}

		public void CopyTo(IDoklad[] array, int arrayIndex) {
			throw new NotImplementedException();
		}

		public IDoklad Get(IDoklad item) {
			throw new NotImplementedException();
		}

		IEnumerator<IDoklad> IDoklady.GetEnumerator() {
			throw new NotImplementedException();
		}

		public bool Remove(IDoklad item) {
			throw new NotImplementedException();
		}

		IDoklad IDoklady.this[int index] {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
