using System.Text;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence
{
	public class Pramen : IPredloha, IPramen
	{
		private Datace mdtcDatace = new Datace();
		private Ulozeni mulUlozeni = new Ulozeni();

		/// <summary>
		/// Uzuální zkratka pramene
		/// </summary>
		public string Zkratka { get; set; }



		public Ulozeni Ulozeni {
			get { return ((IPramen)this).Ulozeni as Ulozeni; }
			set { ((IPramen)this).Ulozeni = value; }
		}


		/// <summary>
		/// Uložení tisku
		/// </summary>
		IUlozeni IPramen.Ulozeni
		{
			get { return mulUlozeni; }
			set
			{
				if(mulUlozeni == null)
					mulUlozeni = new Ulozeni();
				mulUlozeni = (Ulozeni) value;
			}
		}

		
		public Datace Datace {
			get { return ((IPramen)this).Datace as Datace; }
			set { ((IPramen)this).Datace = value; }
		}


		/// <summary>
		/// Datace tisku
		/// </summary>
		IDatace IPramen.Datace
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


		#region IPramen Members


		public TypPredlohy Typ {
			get {
				return TypPredlohy.Rukopis;
			}
			set {
				//TODO dodělat nastavení typu; může být i něco jiného než rukopis?
			}
		}

		public virtual string Popis() {
			//TODO dodělat popis
			StringBuilder sb = new StringBuilder(300);
			if (Ulozeni != null)
			{
				sb.AppendFormat("{0}", Ulozeni.Popis());
			}
			return sb.ToString();
		}

		#endregion
	}
}
