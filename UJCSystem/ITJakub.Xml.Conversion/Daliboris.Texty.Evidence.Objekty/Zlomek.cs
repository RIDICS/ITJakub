using System;

namespace Daliboris.Texty.Evidence
{

	public class CasovyZlomek {
		public CasovyZlomek() { }
		public CasovyZlomek(string strPopis) {
			AnalyzujPopis(strPopis);
		}
		public CasovyZlomek(string strCitatel, string strJmenovatel) { }

		/// <summary>
		/// Slovní vyjádření čitatele, např. 1., 4., 60.
		/// </summary>
		public string CitatelPopis { get; set; }

		/// <summary>
		/// Slovní vyjáídření jemnovatele, např. polovina, čtvrtina, léta
		/// </summary>
		public string JmenovatelPopis { get; set; }
		public int Citatel { get; set; }
		public int Jmenovatel { get; set; }

		private void AnalyzujPopis(string sPopis) {

		}

		public static int CitatelPopisNaCislo(string sPopis) {
			int iCitatel;
			if (Int32.TryParse(sPopis, out iCitatel)) {
				if (iCitatel > 10)
					return iCitatel / 10;
				return iCitatel;
			}
			else
				return -1;
		}
		public static int JmenovatelPopisNaCislo(string sPopis) {
			switch (sPopis) {
				case AnalyzatorDatace.csPolovina:
				case AnalyzatorDatace.csJenPolovina:
					return 2;
				case AnalyzatorDatace.csCtvrtina:
					return 4;
				case AnalyzatorDatace.csTretina:
					return 3;
				case AnalyzatorDatace.csLeta:
					return 10;
				default:
					return -1;
			}
		}

	}
}