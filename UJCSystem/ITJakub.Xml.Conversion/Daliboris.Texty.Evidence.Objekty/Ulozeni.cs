using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence
{
	public class Ulozeni : IUlozeni
	{

		public Ulozeni() {}
		public Ulozeni(string  strZeme, string strMesto, string  strInstituce, string strSignatura, string strFoliacePaginace)
		{
			Zeme = strZeme;
			Mesto = strMesto;
			Instituce = strInstituce;
			Signatura = strSignatura;
			FoliacePaginace = strFoliacePaginace;
		}

		/// <summary>
		/// Země uložení rukopisu/tisku
		/// </summary>
		public string Zeme { get; set; }
		/// <summary>
		/// Město uložení rukopisu/tisku
		/// </summary>
		public string Mesto { get; set; }
		/// <summary>
		/// Instituce uložení rukopisu/tisku
		/// </summary>
		public string Instituce { get; set; }
		/// <summary>
		/// Signatura rukopisu/tisku
		/// </summary>
		public string Signatura { get; set; }
		/// <summary>
		/// Foliace/paginace rukopisu/tisku
		/// </summary>
		public string FoliacePaginace { get; set; }

		public string Popis()
		{
          StringBuilder sb = new StringBuilder();
          if (!String.IsNullOrEmpty(this.Instituce))
              sb.Append(this.Instituce+ ", ");
          if (!String.IsNullOrEmpty(this.Mesto))
              sb.Append(this.Mesto+ ", ");
          if (!String.IsNullOrEmpty(this.Zeme))
              sb.Append(this.Zeme);
			 if (!String.IsNullOrEmpty(this.Signatura))
				 sb.Append(", sign.: " + this.Signatura);
			 if (this.FoliacePaginace != null)
				 sb.Append(", " + this.FoliacePaginace);
          return sb.ToString();
		}

	}
}
