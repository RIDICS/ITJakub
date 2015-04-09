using System;

namespace Daliboris.OOXML.Word
{
	public class ProgressEventArgs : EventArgs {
		private int mintCelkemZnaku;
		private int mintAktualniPocetZnaku;

		public ProgressEventArgs() { }
		public ProgressEventArgs(int intCharCount) {
			mintCelkemZnaku = intCharCount;
		}
		public int CharCount {
			get { return mintCelkemZnaku; }
			set { mintCelkemZnaku = value; }
		}
		public int CurrentChar {
			get { return mintAktualniPocetZnaku; }
			set { mintAktualniPocetZnaku = value; }
		}
		public decimal Percent {
			get {
				if (mintAktualniPocetZnaku == 0)
					return 0;
				return Decimal.Divide(mintAktualniPocetZnaku, mintCelkemZnaku) * 100;
				//return (double)(mintAktualniPocetZnaku / mintCelkemZnaku) * 100;
			}
		}
		public override string ToString() {
			return String.Format("{0} %", this.Percent);
		}
	}
}