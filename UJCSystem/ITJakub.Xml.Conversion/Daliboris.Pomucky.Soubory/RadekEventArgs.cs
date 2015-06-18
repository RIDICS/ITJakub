using System;

namespace Daliboris.Pomucky.Soubory {
	public class RadekEventArgs : EventArgs {
		public string Text { get; set; }
		public int Poradi { get; set; }
		public RadekEventArgs(string strText, int intPoradi) {
			this.Text = strText;
			this.Poradi = intPoradi;
		}
	}
}
