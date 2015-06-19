using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence
{
	public class Tisk : Pramen, ITisk
	{
		/// <summary>
		/// Tiskař díla
		/// </summary>
		public string Tiskar { get; set; }
		/// <summary>
		/// Místo tisku díla
		/// </summary>
		public string MistoTisku { get; set; }
		/// <summary>
		/// Číslo knihopisu
		/// </summary>
		public string Knihopis { get; set; }

		public override string Popis()
		{
			StringBuilder sb = new StringBuilder();

			if (this.MistoTisku != null)
			{
				sb.Append(this.MistoTisku);
				if (base.Datace.SlovniPopis != null)
					sb.Append(", " + base.Datace.SlovniPopis + "; ");
			}
			if (this.Tiskar != null)
				sb.Append(this.Tiskar);
			if (this.Knihopis != null)
				sb.Append("; Knihopis: " + this.Knihopis);
			return sb.ToString();
		}

	}
}
