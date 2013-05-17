using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models.CrosswordsApp {
    class CrosswordChar : CrosswordField {

        private string content;

        public CrosswordChar(string c) {
            content = c;
        }

        internal override string getText() {
            return content;
        }

    }
}
