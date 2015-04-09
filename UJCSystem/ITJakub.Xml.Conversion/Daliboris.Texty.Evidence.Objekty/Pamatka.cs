using System;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence
{
	public class Pamatka : IPamatka
	{
		private Datace mdtcDatace = new Datace();
		private Pramen mprmPramen = new Pramen();
		private Edice medcEdice = new Edice();

		public Pamatka(){}

		public Pamatka(string strAutor, string strTitul,
		               string strZkratka, string strLiterarniDruh, 
									string  strLiterarniZanr)
		{
			Autor = strAutor;
			Titul = strTitul;
			Zkratka = strZkratka;
			LiterarniDruh = strLiterarniDruh;
			LiterarniZanr = strLiterarniZanr;
		}

		/// <summary>
		/// Autor díla
		/// </summary>
		public string Autor { get; set; }
		/// <summary>
		/// Titul díla
		/// </summary>
		/// <summary>
		/// Je-li autor díla uzuální, uvádí se v hranatých závorkách
		/// </summary>
		public bool UzualniAutor
		{
			get
			{
				if (String.IsNullOrEmpty(this.Autor))
					return false;
				else
					return (this.Autor.Contains("["));
			}
		}

		/// <summary>
		/// Titul díla
		/// </summary>
		public string Titul { get; set; }
		/// <summary>
		/// Je-li titul díla uzuální, uvádí se v hranatých závorkách
		/// </summary>
		public bool UzualniTitul
		{
			get
			{
				if (String.IsNullOrEmpty(this.Titul))
					return false;
				else
					return (this.Titul.Contains("["));
			}
		}


		/// <summary>
		/// Uzuální zkratka památky
		/// </summary>
		public string Zkratka { get; set; }

		/// <summary>
		/// Literární druh popisující památku
		/// <example>próza, verš</example>
		/// </summary>
		public string LiterarniDruh { get; set; }

		/// <summary>
		/// Literární žánr památky
		/// <example>cestopis</example>
		/// </summary>
		public string LiterarniZanr { get; set; }

		public Datace Datace
		{
			get { return ((IPamatka) this).Datace as Datace; }
			set { ((IPamatka) this).Datace = value; }
		}

		/// <summary>
		/// Datace díla
		/// </summary>
		IDatace IPamatka.Datace
		{
			get { return mdtcDatace; }
			set
			{
				if (mdtcDatace == null)
				{
					mdtcDatace = new Datace();
				}
				mdtcDatace = (Datace) value;
			}
		}

		public Pramen Pramen
		{
			get { return ((IPamatka) this).Pramen as Pramen; }
			set { ((IPamatka) this).Pramen = value; }
		}

		/// <summary>
		/// Pramen památky
		/// </summary>
		IPramen IPamatka.Pramen
		{
			get
			{
				return mprmPramen;
			}
			set
			{
				if (mprmPramen == null)
					mprmPramen = new Pramen();
				mprmPramen = (Pramen) value;
			}
		}
		public Edice Edice
		{
			get { return ((IPamatka) this).Edice as Edice; }
			set { ((IPamatka) this).Edice = value; }

		}

		/// <summary>
		/// Edice památky (nebo spíše pramene)
		/// </summary>
		IEdice IPamatka.Edice
		{
			get
			{
				return medcEdice;
			}
			set
			{
				if (medcEdice == null)
				{
					medcEdice = new Edice();
				}
				medcEdice = (Edice) value;
			}
		}




	}
}
