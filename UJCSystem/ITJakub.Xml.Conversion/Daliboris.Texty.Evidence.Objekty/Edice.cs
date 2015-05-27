using System.Text;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence
{
	public class Edice : IEdice
	{

		private Editori medtEditori = new Editori();


		public Editori Editori {
			get { return ((IEdice)this).Editori as Editori; }
			set { ((IEdice)this).Editori = value; }
		}

		/// <summary>
		/// Titul edice
		/// </summary>
		public string Titul { get; set; }



		/// <summary>
		/// Editoři edice
		/// </summary>
		IEditori IEdice.Editori
		{
			get { return medtEditori; }
			set { medtEditori = (Editori) value; }
		}
		/// <summary>
		/// Místo vydání edice
		/// </summary>
		public string MistoVydani { get; set; }
		/// <summary>
		/// Rok vydání edice
		/// </summary>
		public string RokVydani { get; set; }
		/// <summary>
		/// Strany edice
		/// </summary>
		public string Strany { get; set; }

		public string Popis()
		{
			StringBuilder sb = new StringBuilder();
			if (this.Titul != null)
			{
				sb.Append(this.Titul + ", ");
			}
			if (this.MistoVydani != null)
			{
				sb.Append(this.MistoVydani);
			}
			if (this.RokVydani != null)
			{
				sb.Append(" " + this.RokVydani);
			}
			if (this.Strany != null)
			{
				sb.Append(", s. " + this.Strany + ", ");
			}
			if (this.Editori.Popis() != null)
			{
				sb.Append("ed. " + this.Editori.Popis());
			}
			return sb.ToString();
			//return base.Popis();
		}

		#region IPredloha Members

		public TypPredlohy Typ {
			get {
				return TypPredlohy.Edice; 
			}
			set {}
		}

		#endregion
	}
}
