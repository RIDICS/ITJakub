using System.Collections.Generic;

namespace Daliboris.Statistiky.Frekvence {
	public class PocitadloFrekvence {
		private char[] machPocitaneZnaky = null;
		private Dictionary<FrekvenceZnaku, string> mgdcFrekvenceSTextem = new Dictionary<FrekvenceZnaku, string>();

		public PocitadloFrekvence() { }
		public PocitadloFrekvence(char[] achPocitaneZnaky) {
			machPocitaneZnaky = achPocitaneZnaky;
		}


	}
}
