using System;
using System.Collections;
using System.Collections.Generic;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence
{
	public class Editori : List<Editor>, IEditori
	{
		private const bool cblnIsReadOnly = false;

		public string Popis()
		{
			const string csSpojeni = " – ";
			string sText = null;
			foreach (IEditor editor in this)
			{
				sText += editor.Jmeno +  csSpojeni;
			}
			if (sText != null)
			{
				sText = sText.Substring(0, sText.Length - csSpojeni.Length);
			}
			return sText;
		}


		/*
		private List<Editor> mglsEditori = new List<Editor>();
		private bool isReadOnly = false;

		public string Popis()
		{
			const string csSpojeni = " – ";
			string sText = null;
			foreach (Editor editor in mglsEditori)
			{
				sText += editor.Jmeno +  csSpojeni;
			}
			if (sText != null)
			{
				sText = sText.Substring(0, sText.Length - csSpojeni.Length);
			}
			return sText;
		}

		#region Implementation of IEnumerable

		public IEnumerator<Editor> GetEnumerator()
		{
			return mglsEditori.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Implementation of ICollection<Editor>

		public void Add(Editor item)
		{
			mglsEditori.Add(item);
		}

		public void Clear()
		{
			mglsEditori.Clear();
		}

		public bool Contains(Editor item)
		{
			return mglsEditori.Contains(item);
		}

		public void CopyTo(Editor[] array, int arrayIndex)
		{
			mglsEditori.CopyTo(array, arrayIndex);
		}

		public bool Remove(Editor item)
		{
			return mglsEditori.Remove(item);
		}

		public int Count
		{
			get { return mglsEditori.Count; }
		}

		public bool IsReadOnly
		{
			get { return isReadOnly; }
		}

		#endregion
		*/



		#region IEditori Members

		string IEditori.Popis() {
			return this.Popis();
		}

		#endregion

		#region IList<IEditor> Members

		int IList<IEditor>.IndexOf(IEditor item) {
			return this.IndexOf(item as Editor);
		}

		void IList<IEditor>.Insert(int index, IEditor item) {
			this.Insert(index, item as Editor);
		}

		void IList<IEditor>.RemoveAt(int index) {
			this.RemoveAt(index);
		}

		IEditor IList<IEditor>.this[int index] {
			get {
				return this[index];
			}
			set {
				this[index] = value as Editor;
			}
		}

		#endregion

		#region ICollection<IEditor> Members

		void ICollection<IEditor>.Add(IEditor item) {
			this.Add(item as Editor);
		}

		void ICollection<IEditor>.Clear() {
			this.Clear();
		}

		bool ICollection<IEditor>.Contains(IEditor item) {
			return this.Contains(item as Editor);
		}

		void ICollection<IEditor>.CopyTo(IEditor[] array, int arrayIndex) {
			this.CopyTo(array as Editor[], arrayIndex);
		}

		int ICollection<IEditor>.Count {
			get { return this.Count; }
		}

		bool ICollection<IEditor>.IsReadOnly {
			get { return cblnIsReadOnly; }
		}

		bool ICollection<IEditor>.Remove(IEditor item) {
			return this.Remove(item as Editor);
		}

		#endregion

		#region IEnumerable<IEditor> Members

		IEnumerator<IEditor> IEnumerable<IEditor>.GetEnumerator() {
			return this.ConvertAll(new Converter<Editor, IEditor>(Editor2IEditor)).GetEnumerator();
		}

		private static IEditor Editor2IEditor(Editor input)
		{
			return input;
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		#endregion
	}
}