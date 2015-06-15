using System;
using System.Collections.Generic;
using Daliboris.Texty.Export.Rozhrani;

namespace Daliboris.Texty.Export {
	public class TransformaceNastaveni : ITransformaceNastaveni
	{

		public string VstupniSlozka { get; set; }
		public string MaskaSoubru { get; set; }
		public string VystupniSlozka { get; set; }
		public string DocasnaSlozka { get; set; }
		public string SlozkaXslt { get; set; }
		public bool SmazatDocasneSoubory { get; set; }
		public List<ITransformacniKrok> TransformacniKroky { get; set; }
    public string TransformacniSoubor { get; set; }
		public string TranskripcniSoubor { get; set; }
		public DateTime CasExportu { get; set; }
	}
}
