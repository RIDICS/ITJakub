using System.Collections.Generic;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence {
	public class Exporty : List<Export> {
		public Exporty() {}
		public Exporty(IEnumerable<IExport> glExporty)
		{
			foreach (IExport export in glExporty)
			{
				this.Add(new Export(export.ZpusobVyuziti, export.CasExportu, export.KontrolniSoucet));
			}
		}
	}
}
