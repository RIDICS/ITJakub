using System;
using System.Collections.Generic;
using Daliboris.Texty.Evidence.Rozhrani;
using Daliboris.Texty.Export.Rozhrani;

namespace Daliboris.Texty.Export
{
	public abstract class ExportNastaveni : IExportNastaveni
	{
		public string VstupniSlozka { get; set; }
		public string MaskaSoubru { get; set; }
		public string VystupniSlozka { get; set; }
		public string DocasnaSlozka { get; set; }
		public IList<IPrepis> Prepisy { get; set; }
		public string SouborMetadat { get; set; }
		public string ValidacniXsd { get; set; }
		public string SlozkaXslt { get; set; }
		public bool SmazatDocasneSoubory { get; set; }
		public List<ITransformacniKrok> TransformacniKroky { get; set; }
		public string TransformacniSoubor { get; set; }
		public string TranskripcniSoubor { get; set; }
		public DateTime CasExportu { get; set; }
		public bool Evidovat { get; set; }
		public bool VypisovatVse { get; set; }

		/// <summary>
		/// Soubor popisující Xslt transformace využívané knihovnou XsltTransformation.
		/// </summary>
		public string SouborTransformaci { get; set; }
	}
}
