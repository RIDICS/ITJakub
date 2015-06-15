using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence
{
	public class Hlava : IHlava
	{
		private IPamatka mpamPamatka;
		private IEditori medtEditori;
		private IPredloha mprdPredloha;

		/// <summary>
		/// Památka zachycená v přepisu
		/// </summary>
		public Pamatka Pamatka { 
			get
			{
				if (mpamPamatka == null)
				{
					mpamPamatka = new Pamatka();
					mpamPamatka.Pramen = new Pramen();
				}
				return (Pamatka) mpamPamatka;
			}
			set
			{
				if (mpamPamatka == null)
				{
					mpamPamatka = new Pamatka();
					mpamPamatka.Pramen = new Pramen();
				}
				mpamPamatka  = value;
			}
		}

		/// <summary>
		/// Předloha sloužící pro přepis (rukopis, tisk, edice)
		/// </summary>
		public Predloha Predloha
		{
			get
			{
				if (mprdPredloha == null)
				{
					mprdPredloha = new Predloha();
				}

				return (Predloha) mprdPredloha;
			}
			set
			{
				if (mprdPredloha == null)
				{
					mprdPredloha = new Predloha();
				}
				mprdPredloha = value;
			}
		}

		/// <summary>
		/// Editoři elektronického přepisu
		/// </summary>

		public Editori Editori { 
			get
			{
				if (medtEditori == null)
				{
					medtEditori = new Editori();
				}
				return (Editori) medtEditori;
			}
			set
			{
				if (medtEditori == null)
				{
					medtEditori = new Editori();
				}
				medtEditori = value;
			}
		}
		/// <summary>
		/// Poznámka editorů k přepisu
		/// </summary>
		public string Poznamka { get; set; }

		/// <summary>
		/// Textová ukázka přepisu
		/// </summary>
		public string Ukazka { get; set; }


		#region IHlava Members


		IEditori IHlava.Editori {
			get {
				return Editori;
			}
			set {
				Editori = value as Editori;
			}
		}

		#endregion

		#region IHlava Members

		IPamatka IHlava.Pamatka {
			get {
			 return	Pamatka;
			}
			set {
				Pamatka = value as Pamatka;
			}
		}

		#endregion

		#region IHlava Members


		IPredloha IHlava.Predloha {
			get {
				return Predloha;
			}
			set {
				Predloha = value as Predloha;
			}
		}

		#endregion
	}
}
