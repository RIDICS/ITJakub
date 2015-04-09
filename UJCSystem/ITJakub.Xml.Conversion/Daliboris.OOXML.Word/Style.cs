using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Daliboris.OOXML.Word {
	public class Style {
		public string ID { get; set; }
		public string Name { get; set; }
		public string UIName { get; set; }
		public bool IsCustom { get; set; }
		public bool IsDefault { get; set; }
		public bool IsUsed { get; set; }
		//je potřeba evidovat i "podkladový" styl; u nějž může být nastaven jazyk
		public string Language { get; set; }
		public StyleType Type { get; set; }
		public string BasenOnStyleID { get; set; }
		public Style BasedOnStyle { get; set; }
		public bool? NoProof { get; set; }
		/// <summary>
		/// Počet výskytů daného stylu v dokumentu
		/// </summary>
		public int Count { get; set; }

        /// <summary>
        /// formátování stylu
        /// </summary>
   public IRunFormatting RunFormatting { get; set; }

		/// <summary>
		/// Vypočítané formátování stylu  (sloučené s formátování základního stylu)
		/// </summary>
	 public IRunFormatting ComputedRunFormatting { get; set; }

	 public Style()
	 {
		 RunFormatting = new RunFormatting();
		 ComputedRunFormatting = new RunFormatting();
	 }

	 public void ComputeFormatting() {
		 if (BasedOnStyle != null)
		 {
			 BasedOnStyle.ComputeFormatting();
		 }
		 if (BasedOnStyle == null)
			 ComputedRunFormatting = RunFormatting;
		 else
			 ComputedRunFormatting = BasedOnStyle.RunFormatting.MergeFormatting(RunFormatting);
	 }

	}
}
