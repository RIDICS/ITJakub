using Campari.Software.Reflection;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence {
	using System;
	using System.Xml.Serialization;

	/// <summary>
	/// Informace o proběhnuvším exportu přepisu do pro další využití
	/// </summary>
	public class Export : IEquatable<IExport>, IExport
	{
		private ZpusobVyuziti mzpvZpusobVyuziti;
		private DateTime mdtCasExportu;


		public Export() { }

	    public Export(ZpusobVyuziti mzpvZpusobVyuziti, DateTime mdtCasExportu, string kontrolniSoucet) : this(mzpvZpusobVyuziti, mdtCasExportu)
	    {
	        KontrolniSoucet = kontrolniSoucet;
	    }

	    public Export(ZpusobVyuziti zpvZpusobVyuziti, DateTime dtCasExportu) {
			mzpvZpusobVyuziti = zpvZpusobVyuziti;
			mdtCasExportu = dtCasExportu;
		}
		public ZpusobVyuziti ZpusobVyuziti {
			get { return mzpvZpusobVyuziti; }
			set { mzpvZpusobVyuziti = value; }
		}
		public DateTime CasExportu { get { return mdtCasExportu; } set { mdtCasExportu = value; } }
	    public string KontrolniSoucet { get; set; }

	    public override string ToString() {
			return mzpvZpusobVyuziti.GetDescription() + " – " + mdtCasExportu.ToShortDateString() + " " + mdtCasExportu.ToShortTimeString();
		}

		[XmlIgnore]
		public string NazevSouboru { get; set; }

		#region IEquatable<Export> Members

		public bool Equals(IExport other) {
			return (mzpvZpusobVyuziti == other.ZpusobVyuziti && mdtCasExportu == other.CasExportu);
		}

		#endregion
	}
}
