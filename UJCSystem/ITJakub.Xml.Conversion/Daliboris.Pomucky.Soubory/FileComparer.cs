using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

namespace Daliboris.Pomucky.Soubory {

	//http://webdevel.blogspot.com/2007/09/sorting-files-by-name-date-filesize-etc.html
	/// <summary>
	/// Třía pro porovnávání souborů podle jednotlivých atributů
	/// </summary>
	public class FileComparer : IComparer {
		public enum CompareBy {
			Name /* a-z */,
			LastWriteTime /* oldest to newest */,
			CreationTime  /* oldest to newest */,
			LastAccessTime /* oldest to newest */,
			FileSize /* smallest first */
		}

		// default comparison
		int _CompareBy = (int)CompareBy.Name;

		public enum SortOrder
		{
			Ascend,
			Descend
		}

		private SortOrder msrRazeni = SortOrder.Ascend;

		public FileComparer() {
		}

		public FileComparer(CompareBy cmbCompareBy)
		{
			_CompareBy = (int)cmbCompareBy;
		}

		public FileComparer(CompareBy compareBy, SortOrder sroSortOrder) {
			_CompareBy = (int)compareBy;
			msrRazeni = sroSortOrder;
		}

		int IComparer.Compare(object x, object y) {
			int output = 0;
			FileInfo file1 = new FileInfo(x.ToString());
			FileInfo file2 = new FileInfo(y.ToString());
			switch (_CompareBy) {
				case (int)CompareBy.LastWriteTime:
					output = DateTime.Compare(file1.LastWriteTime, file2.LastWriteTime);
					break;
				case (int)CompareBy.CreationTime:
					output = DateTime.Compare(file1.CreationTime, file2.CreationTime);
					break;
				case (int)CompareBy.LastAccessTime:
					output = DateTime.Compare(file1.LastAccessTime, file2.LastAccessTime);
					break;
				case (int)CompareBy.FileSize:
					output = Convert.ToInt32(file1.Length - file2.Length);
					break;
				case (int)CompareBy.Name:
				default:
					output = (new CaseInsensitiveComparer()).Compare(file1.Name, file2.Name);
					break;
			}
			if(msrRazeni == SortOrder.Descend)
				output = -output;
			return output;
		}
	}
}
