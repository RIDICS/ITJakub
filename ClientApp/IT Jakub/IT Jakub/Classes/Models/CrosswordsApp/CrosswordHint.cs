using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models.CrosswordsApp {
    class CrosswordHint : CrosswordField {

        private string hint_x;
        private string hint_y;

        public CrosswordHint(string hint_v, string hint_h) {
            hint_x = hint_h;
            hint_y = hint_v;
        }

        internal override string getText() {
            return '[' + hint_x + "]:[" + hint_y + ']';
        }

        internal string getHorizontalHint() {
            return hint_x;
        }

        internal string getVerticalHint() {
            return hint_y;
        }

    }
}
