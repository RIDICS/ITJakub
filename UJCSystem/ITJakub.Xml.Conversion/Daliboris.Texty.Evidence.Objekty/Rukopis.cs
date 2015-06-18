using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence
{
	public class Rukopis : Pramen, IRukopis
	{

/*		private Datace mdtcDatace = new Datace();
		private Ulozeni mulUlozeni = new Ulozeni();
		/// <summary>
		/// Datace pramene
		/// </summary>
		public Datace Datace
		{
			get { return mdtcDatace; }
			set { mdtcDatace = value; }
		}

		/// <summary>
		/// Uložení pramene
		/// </summary>
		public Ulozeni Ulozeni
		{
			get { return mulUlozeni; }
			set { mulUlozeni = value; }
		}*/

		public override string Popis()
		{
			return base.Ulozeni.Popis();
			//return base.Popis();
		}

	}
}
